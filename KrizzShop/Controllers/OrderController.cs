using Braintree;
using KrizzShop_DataAccess.Repository.IRepository;
using KrizzShop_Models;
using KrizzShop_Models.ViewModels;
using KrizzShop_Utility;
using KrizzShop_Utility.BrainTree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KrizzShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _orderHeaderRepo;
        private readonly IOrderDetailRepository _orderDetailRepo;
        private readonly IBrainTreeGate _brain;
        public OrderController(IOrderHeaderRepository orderHeaderRepo, IOrderDetailRepository orderDetailRepo, IBrainTreeGate brain)
        {
            _orderHeaderRepo = orderHeaderRepo;
            _orderDetailRepo = orderDetailRepo;
            _brain = brain;
        }

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        [HttpGet]
        public IActionResult Index(string searchName = null,
                                   string searchEmail = null,
                                   string searchPhone = null,
                                   string Status = null)
        {
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderHeaderList = _orderHeaderRepo.GetAll(),
                StatusList = WC.listStatus.ToList().Select(i => new SelectListItem()
                {
                    Text = i,
                    Value = i
                })
            };

            if (!string.IsNullOrEmpty(searchName))
            {
                orderListVM.OrderHeaderList = orderListVM.OrderHeaderList
                    .Where(n => n.FullName.ToLower().Contains(searchName.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVM.OrderHeaderList = orderListVM.OrderHeaderList
                    .Where(n => n.Email.ToLower().Contains(searchEmail.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListVM.OrderHeaderList = orderListVM.OrderHeaderList
                    .Where(n => n.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
            }

            if (!string.IsNullOrEmpty(Status) && Status != "--Order Status--")
            {
                orderListVM.OrderHeaderList = orderListVM.OrderHeaderList
                    .Where(n => n.OrderStatus.ToLower().Equals(Status.ToLower()));
            }

            return View(orderListVM);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _orderHeaderRepo.Find(id),
                OrderDetails = _orderDetailRepo.GetAll(
                    filter: d => d.OrderHeader.Id == id,
                    includeProperties: $"{WC.ProductName}")
            };

            return View(OrderVM);
        }

        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeaderFromDb = _orderHeaderRepo
                .Find(OrderVM.OrderHeader.Id);

            orderHeaderFromDb.FullName = OrderVM.OrderHeader.FullName;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            orderHeaderFromDb.Email = OrderVM.OrderHeader.Email;

            _orderHeaderRepo.Update(orderHeaderFromDb);
            _orderHeaderRepo.Save();

            TempData[WC.Success] = "Order Details Updated Successfully";

            return RedirectToAction(nameof(Details), "Order", new {id = orderHeaderFromDb.Id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing()
        {
            OrderHeader orderHeader = _orderHeaderRepo
                .Find(OrderVM.OrderHeader.Id);

            orderHeader.OrderStatus = WC.StatusInProcess;
            _orderHeaderRepo.Save();

            TempData[WC.Success] = "Order Is In Process";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _orderHeaderRepo
                .Find(OrderVM.OrderHeader.Id);

            orderHeader.OrderStatus = WC.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            _orderHeaderRepo.Save();

            TempData[WC.Success] = "Order Shipped Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _orderHeaderRepo
                .Find(OrderVM.OrderHeader.Id);

            var gateway = _brain.GetGateway();
            Transaction transaction = gateway.Transaction.Find(orderHeader.TransactionId);

            if(transaction.Status == TransactionStatus.AUTHORIZED ||
               transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                //no refound
                Result<Transaction> resultVoid = gateway.Transaction.Void(orderHeader.TransactionId);
            }
            else
            {
                Result<Transaction> resultRefund = gateway.Transaction.Refund(orderHeader.TransactionId);
            }

            orderHeader.OrderStatus = WC.StatusCancelled;
            _orderHeaderRepo.Save();

            TempData[WC.Success] = "Order Cancelled Successfully";
            
            return RedirectToAction(nameof(Index));
        }
    }
}

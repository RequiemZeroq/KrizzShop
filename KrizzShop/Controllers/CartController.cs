using Braintree;
using KrizzShop_DataAccess.Repository.IRepository;
using KrizzShop_Models;
using KrizzShop_Models.ViewModels;
using KrizzShop_Utility;
using KrizzShop_Utility.BrainTree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Security.Claims;
using System.Text;

namespace KrizzShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IProductRepository _productRepo;
        private readonly IInquiryDetailRepository _inquiryDetailRepo;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepo;
        private readonly IOrderHeaderRepository _orderHeaderRepo;
        private readonly IOrderDetailRepository _orderDetailRepo;
        private readonly IBrainTreeGate _brain;

        public CartController(IWebHostEnvironment webHostEnvironment,
                              IEmailSender emailSender,
                              IApplicationUserRepository userRepo,
                              IProductRepository productRepo,
                              IInquiryDetailRepository inquiryDetailRepo,
                              IInquiryHeaderRepository inquiryHeaderRepo,
                              IOrderHeaderRepository orderHeaderRepo,
                              IOrderDetailRepository orderDetailRepo,
                              IBrainTreeGate brain)
        {
            _userRepo = userRepo;
            _productRepo = productRepo;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _inquiryDetailRepo = inquiryDetailRepo;
            _inquiryHeaderRepo = inquiryHeaderRepo;
            _orderHeaderRepo = orderHeaderRepo;
            _orderDetailRepo = orderDetailRepo;
            _brain = brain;
        }

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        [HttpGet]
        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) is not null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0) 
            {
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart)
                    .ToList();
            }
            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId)
                .ToList();
            IEnumerable<Product> prodListTemp = _productRepo.GetAll(
                filter: p => prodInCart.Contains(p.Id));
            IList<Product> prodList = new List<Product>();

            foreach(var cartObj in shoppingCartList)
            {
                Product prodTemp = prodListTemp
                    .FirstOrDefault(u => u.Id == cartObj.ProductId);

                prodTemp.TempQuantity = cartObj.Quantity;
                prodList.Add(prodTemp);
            }

            return View(prodListTemp);  
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPost(IEnumerable<Product> prodList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            foreach (var product in prodList)
            {
                shoppingCartList.Add(new ShoppingCart
                {
                    ProductId = product.Id,
                    Quantity = product.TempQuantity
                });
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Summary));
        }

        [HttpPost, ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(IFormCollection collection, ProductUserVM ProductUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (User.IsInRole(WC.AdminRole))
            {
                OrderHeader orderHeader = new OrderHeader()
                {
                    CreatedByUserId = claim.Value,
                    FinalOrderTotal = ProductUserVM.ProductList.Sum(x => x.TempQuantity *  x.Price),
                    City = ProductUserVM.ApplicationUser.City,
                    StreetAddress = ProductUserVM.ApplicationUser.StreetAddress,
                    State = ProductUserVM.ApplicationUser.State,
                    PostalCode = ProductUserVM.ApplicationUser.PostalCode,
                    FullName = ProductUserVM.ApplicationUser.FullName,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                    OrderDate = DateTime.Now,
                    OrderStatus = WC.StatusPending,
                    Email = ProductUserVM.ApplicationUser.Email,
                };

                _orderHeaderRepo.Add(orderHeader);
                _orderHeaderRepo.Save();

                foreach(var product in ProductUserVM.ProductList)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                         OrderHeaderId = orderHeader.Id,
                         PricePerPiece = product.Price,
                         Quantity = product.TempQuantity,
                         ProductId = product.Id
                    };
                    _orderDetailRepo.Add(orderDetail);
                }

                _orderDetailRepo.Save();
                TempData[WC.Success] = "Order created successfully";

                string nonceFromTheClient = collection["payment_method_nonce"];

                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),
                    PaymentMethodNonce = nonceFromTheClient,
                    OrderId = orderHeader.Id.ToString(),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                var gateway = _brain.GetGateway();
                Result<Transaction> result = gateway.Transaction.Sale(request);

                if(result.Target.ProcessorResponseText == "Approved")
                {
                    orderHeader.TransactionId = result.Target.Id;
                    orderHeader.OrderStatus = WC.StatusApproved;
                }
                else
                {
                    orderHeader.OrderStatus = WC.StatusCancelled;
                }

                _orderHeaderRepo.Save();

                return RedirectToAction(nameof(InquiryConfiramtion), new { id = orderHeader.Id });
            }
            else
            {
                var pathToTemplate =
                _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar + "templates" +
                Path.DirectorySeparatorChar + "Inquiry.html";

                var subject = "New Inquiry";
                string htmlBody = string.Empty;

                using (StreamReader sr = System.IO.File.OpenText(pathToTemplate))
                {
                    htmlBody = sr.ReadToEnd();
                }

                var productListSB = new StringBuilder();
                foreach (var product in ProductUserVM.ProductList)
                {
                    productListSB.Append($" - Name: {product.Name} <span style='font-size:14px'> (ID: {product.Id})</span><br />");
                }
                string messageBody = string.Format(htmlBody,
                    ProductUserVM.ApplicationUser.FullName,
                    ProductUserVM.ApplicationUser.Email,
                    ProductUserVM.ApplicationUser.PhoneNumber,
                    productListSB.ToString());

                await _emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBody);

                InquiryHeader inquiryHeader = new InquiryHeader()
                {
                    ApplicationUserId = claim.Value,
                    FullName = ProductUserVM.ApplicationUser.FullName,
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                    InquiryDate = DateTime.Now
                };
                _inquiryHeaderRepo.Add(inquiryHeader);
                _inquiryHeaderRepo.Save();

                foreach (var product in ProductUserVM.ProductList)
                {
                    InquiryDetail inquiryDetail = new InquiryDetail()
                    {
                        InquiryHeaderId = inquiryHeader.Id,
                        ProductId = product.Id,
                    };

                    _inquiryDetailRepo.Add(inquiryDetail);
                }

                _inquiryDetailRepo.Save();
            }

            return RedirectToAction(nameof(InquiryConfiramtion));
        }

        public IActionResult InquiryConfiramtion(int id = 0)
        {
            OrderHeader orderHeader = _orderHeaderRepo
                .Find(id);

            HttpContext.Session.Clear();

            return View(orderHeader);
        }

        public IActionResult Summary()
        {
            ApplicationUser applicationUser;

            if (User.IsInRole(WC.AdminRole))
            {
                if (HttpContext.Session.Get<int>(WC.SessionInquiryId) != 0)
                {
                    InquiryHeader inquiryHeader = _inquiryHeaderRepo
                        .FirstOrDefault(h => h.Id == HttpContext.Session.Get<int>(WC.SessionInquiryId));

                    applicationUser = new ApplicationUser()
                    {
                        Email = inquiryHeader.Email,
                        PhoneNumber = inquiryHeader.PhoneNumber,
                        FullName = inquiryHeader.FullName,
                    };
                }
                else
                {
                    applicationUser = new ApplicationUser(); 
                }

                var gateway = _brain.GetGateway();
                var clientToken = gateway.ClientToken.Generate();
                ViewBag.ClientToken = clientToken;
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                applicationUser = _userRepo
                    .FirstOrDefault(u => u.Id == claim.Value);
            }


            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) is not null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart)
                    .ToList();
            }
            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId)
                .ToList();
            IEnumerable<Product> prodList = _productRepo.GetAll(
                filter: p => prodInCart.Contains(p.Id)); 

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = applicationUser,
            };

            foreach (var cartObj in shoppingCartList) 
            {
                Product prodTemp = _productRepo
                    .FirstOrDefault(u => u.Id == cartObj.ProductId);
                prodTemp.TempQuantity = cartObj.Quantity;
                ProductUserVM.ProductList.Add(prodTemp);
            }

            return View(ProductUserVM);
        }

        public IActionResult Remove(int? id)
        {
            if(id is null ||
                id == 0)
            {
                return NotFound();
            }

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) is not null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart)
                    .ToList();
            }

            var productToDelete = shoppingCartList
                .FirstOrDefault(i => i.ProductId == id);

            if (productToDelete is not null)
                shoppingCartList.Remove(productToDelete);

            HttpContext.Session.Set<IEnumerable<ShoppingCart>>(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> prodList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            foreach (var product in prodList)
            {
                shoppingCartList.Add(new ShoppingCart
                {
                    ProductId = product.Id,
                    Quantity = product.TempQuantity
                });
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }
    }
}

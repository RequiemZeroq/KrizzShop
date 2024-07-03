using KrizzShop_DataAccess.Repository.IRepository;
using KrizzShop_Models;
using KrizzShop_Models.ViewModels;
using KrizzShop_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KrizzShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inqHeaderRepo;
        private readonly IInquiryDetailRepository _inqDetailRepo;

        [BindProperty]
        public InquiryVM InquiryVM { get; set; }

        public InquiryController(IInquiryDetailRepository inqDRepo,
                                 IInquiryHeaderRepository inqHRepo)
        {
            _inqDetailRepo = inqDRepo;
            _inqHeaderRepo = inqHRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            InquiryVM = new InquiryVM()
            {
                InquiryDetail = _inqDetailRepo.GetAll(
                    filter: d => d.InquiryHeaderId == id, 
                    includeProperties: $"{WC.ProductName}"),
                InquiryHeader = _inqHeaderRepo.FirstOrDefault(
                    filter: h => h.Id == id)
            };

            return View(InquiryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            InquiryVM.InquiryDetail = _inqDetailRepo
                .GetAll(u => u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);
                
            foreach(var detail in InquiryVM.InquiryDetail)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId,
                };
                
                shoppingCartList.Add(shoppingCart); 
            }

            HttpContext.Session.Clear();
            HttpContext.Session.Set<IEnumerable<ShoppingCart>>(WC.SessionCart, shoppingCartList);
            HttpContext.Session.Set<int>(WC.SessionInquiryId, InquiryVM.InquiryHeader.Id);

            return RedirectToAction(nameof(Index), "Cart");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete()
        {
            InquiryHeader inquiryHeader = _inqHeaderRepo.
                FirstOrDefault(
                    filter: h => h.Id == InquiryVM.InquiryHeader.Id);

            IEnumerable<InquiryDetail> inquiryDetails = _inqDetailRepo
                .GetAll(
                    filter: d => d.InquiryHeaderId == InquiryVM.InquiryHeader.Id);

            _inqDetailRepo.RemoveRange(inquiryDetails);
            _inqHeaderRepo.Remove(inquiryHeader);
            _inqHeaderRepo.Save();

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inqHeaderRepo.GetAll() });
        }
        #endregion

    }
}

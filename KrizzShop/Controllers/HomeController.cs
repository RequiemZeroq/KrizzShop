using KrizzShop_DataAccess;
using KrizzShop_Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using KrizzShop_Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using KrizzShop_Utility;
using KrizzShop_DataAccess.Repository.IRepository;
using System.Net.Http.Headers;

namespace KrizzShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;

        public HomeController(ILogger<HomeController> logger,
                              IProductRepository productRepo,
                              ICategoryRepository categoryRepo)
        {
            _logger = logger;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public IActionResult Index()
        {
            var homeVM = new HomeVM()
            {
                Products = _productRepo.GetAll(
                    includeProperties: $"{WC.CategoryName},{WC.ApplicationTypeName}"),
                Categories = _categoryRepo.GetAll()
            };

            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            List<ShoppingCart> shoppingCardList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) is not null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCardList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            DetailsVM detailsVM = new DetailsVM()
            {
                Product = _productRepo.FirstOrDefault(
                    includeProperties: $"{WC.CategoryName},{WC.ApplicationTypeName}",
                    filter: p => p.Id == id), 
                ExistsInCard = false
            };

            if(shoppingCardList.Select(i => i.ProductId).Contains(id)) 
                detailsVM.ExistsInCard = true;

            return View(detailsVM);
        }

        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]  
        public IActionResult DetailsPost(int id, DetailsVM detailsVM)
        {
            List<ShoppingCart> shoppingCardList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) is not null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCardList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            shoppingCardList.Add(new ShoppingCart()
            {
                ProductId = id,
                Quantity = detailsVM.Product.TempQuantity
            });
            HttpContext.Session.Set(WC.SessionCart, shoppingCardList);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCardList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) is not null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCardList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            var itemToRemove = shoppingCardList.SingleOrDefault(r => r.ProductId == id);   
            if(itemToRemove is not null)
                shoppingCardList.Remove(itemToRemove);
            
            HttpContext.Session.Set(WC.SessionCart, shoppingCardList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

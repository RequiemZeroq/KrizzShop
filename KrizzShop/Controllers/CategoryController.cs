using KrizzShop_DataAccess;
using KrizzShop_DataAccess.Repository.IRepository;
using KrizzShop_Models;
using KrizzShop_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KrizzShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _repo;
        public CategoryController(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objList = _repo.GetAll();

            return View(objList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                TempData[WC.Error] = "Error while creating category";
                return View(category);
            }

            _repo.Add(category);
            _repo.Save();
            TempData[WC.Success] = "Category created successfully";
            return RedirectToAction("Index");   
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if(id is null || id == 0)
                return NotFound();

            var category = _repo.Find(id.GetValueOrDefault());

            if (category is null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public IActionResult DeletePost(int id)
        {
            var category = _repo.Find(id);

            if (category is null)
                return NotFound();
            
            _repo.Remove(category);
            _repo.Save();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0)
                return NotFound();

            var category = _repo.Find(id.GetValueOrDefault());

            if(category is null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if(!ModelState.IsValid)
                return View(category);

            _repo.Update(category);
            _repo.Save();

            return RedirectToAction("Index");
        }
    }
}

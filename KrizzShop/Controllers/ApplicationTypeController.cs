using KrizzShop_DataAccess.Repository.IRepository;
using KrizzShop_Models;
using KrizzShop_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KrizzShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly IApplicationTypeRepository _repo;
        public ApplicationTypeController(IApplicationTypeRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objList = _repo.GetAll();
            return View(objList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType applicationType)
        {
            _repo.Add(applicationType);
            _repo.Save();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0)
                return NotFound();

            var appType = _repo.Find(id.GetValueOrDefault());

            if (appType is null)
                return NotFound();

            return View(appType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType applicationType)
        {
            if (!ModelState.IsValid)
                return View(applicationType);

            _repo.Update(applicationType);
            _repo.Save();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if(id is null || id == 0)
                return NotFound();

            var appType = _repo.Find(id.GetValueOrDefault());  

            if(appType is null)
                return NotFound();
            
            return View(appType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id)
        {
            var appType = _repo.Find(id);

            if(appType == null)
                return NotFound();

            _repo.Remove(appType);
            _repo.Save();

            return RedirectToAction(nameof(Index));
        }

    }
}

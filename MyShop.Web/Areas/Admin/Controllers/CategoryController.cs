using Microsoft.AspNetCore.Mvc;
using MyShop.Models;
using MyShop.DataAccess.Data;
using MyShop.Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using MyShop.Utilities;


namespace MyShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var category = _unitOfWork.Category.GetAll();
            return View(category);
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
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Complete();
                TempData["Create"] = "Item has Create Succesfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int? Id)
        {
            if (Id is null | Id == 0)
            {
                return NotFound();
            }

            var categoryId = _unitOfWork.Category.GetFirstorDefaulte(x => x.Id == Id);

            return View(categoryId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Complete();
                TempData["Update"] = "Item has Update Succesfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Delete(int? Id)
        {
            if (Id is null | Id == 0)
            {
                return NotFound();
            }

            var categoryId = _unitOfWork.Category.GetFirstorDefaulte(x => x.Id == Id);

            return View(categoryId);
        }
        [HttpPost]
        public IActionResult DeleteCategory(int? Id)
        {
            if (Id is null | Id == 0)
            {
                return NotFound();
            }

            var categoryId = _unitOfWork.Category.GetFirstorDefaulte(x => x.Id == Id);

            if (categoryId is null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(categoryId);
            _unitOfWork.Complete();
            TempData["Delete"] = "Item has Delete Succesfully";
            return RedirectToAction("Index");
        }

    }
}

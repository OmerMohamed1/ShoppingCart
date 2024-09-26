using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Entities.ViewModels;
using MyShop.Utilities;


namespace MyShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var product = _unitOfWork.Product.GetAll(Includeword: "Category");
            return Json(new { data = product });
        }

        [HttpGet]
        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM productVM, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var RootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(RootPath, @"Images\Porducts");
                    var ext = Path.GetExtension(file.FileName);

                    using (var filestream = new FileStream(Path.Combine(Upload, fileName + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productVM.Product.ImgUrl = @"Images\Porducts\" + fileName + ext;
                }
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Complete();
                TempData["Create"] = "Item has Create Succesfully";
                return RedirectToAction("Index");
            }
            return View(productVM.Product);
        }

        [HttpGet]
        public IActionResult Edit(int? Id)
        {
            if (Id is null | Id == 0)
            {
                return NotFound();
            }


            ProductVM productVM = new ProductVM()
            {
                Product = _unitOfWork.Product.GetFirstorDefaulte(x => x.Id == Id),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {

                var RootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(RootPath, @"Images\Porducts");
                    var ext = Path.GetExtension(file.FileName);

                    if (productVM.Product.ImgUrl != null)
                    {
                        var oldimg = Path.Combine(RootPath, productVM.Product.ImgUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldimg))
                        {
                            System.IO.File.Delete(oldimg);
                        }
                    }

                    using (var filestream = new FileStream(Path.Combine(Upload, fileName + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productVM.Product.ImgUrl = @"Images\Porducts\" + fileName + ext;
                }

                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Complete();
                TempData["Update"] = "Item has Update Succesfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        //[HttpGet]
        //public IActionResult Delete(int? Id)
        //{
        //    if (Id is null | Id == 0)
        //    {
        //        return NotFound();
        //    }

        //    var productId = _unitOfWork.Product.GetFirstorDefaulte(x => x.Id == Id);

        //    return View(productId);
        //}


        [HttpDelete]
        public IActionResult Delete(int? Id)
        {
            if (Id is null | Id == 0)
            {
                return NotFound();
            }

            var productInDb = _unitOfWork.Product.GetFirstorDefaulte(x => x.Id == Id);

            if (productInDb is null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

            _unitOfWork.Product.Remove(productInDb);

            var oldimg = Path.Combine(_webHostEnvironment.WebRootPath, productInDb.ImgUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldimg))
            {
                System.IO.File.Delete(oldimg);
            }

            _unitOfWork.Complete();
            //TempData["Delete"] = "Item has Delete Succesfully";
            return Json(new { success = true, message = "file has been Deleted" });
        }

    }
}

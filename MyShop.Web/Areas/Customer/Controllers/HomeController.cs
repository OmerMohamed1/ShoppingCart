using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Utilities;
using System.Security.Claims;
using X.PagedList;

namespace MyShop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int? page)
        {
            int PageNumber = page ?? 1;
            int PageSize = 4;

            var product = _unitOfWork.Product.GetAll(Includeword: "Category").ToPagedList(PageNumber, PageSize);
            return View(product);
        }

        public IActionResult Details(int Id)
        {
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                ProductId = Id,
                Product = _unitOfWork.Product.GetFirstorDefaulte(x => x.Id == Id, Includeword: "Category"),
                Count = 1
            };

            return View(shoppingCart);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart Cartobj = _unitOfWork.ShoppingCart.GetFirstorDefaulte(
                u => u.ApplicationUserId == claim.Value &&
                            u.ProductId == shoppingCart.Id
                );

            if (Cartobj == null)
            {
                shoppingCart.Id = 0;
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Complete();
                HttpContext.Session.SetInt32(SD.SessionKey,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count()
                    );
            }
            else
            {
                _unitOfWork.ShoppingCart.IncresseCount(Cartobj, shoppingCart.Count);
                _unitOfWork.Complete();
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Search(string? searchString, int page = 1)
        {
            ViewData["CurrentFilter"] = searchString;

            var products = _unitOfWork.Product.GetAll(Includeword: "Category");

            if (!string.IsNullOrEmpty(searchString))
            {
                string lowerSearchString = searchString.ToLower();
                products = products.Where(x => x.Name.ToLower().Contains(lowerSearchString) ||
                                               x.Category.Name.ToLower().Contains(lowerSearchString)).ToList();
            }

            if (!products.Any())
            {
                ViewData["NoResultsMessage"] = "No products found matching your search.";
            }
            else
            {
                ViewData["NoResultsMessage"] = null;
            }

            var pagedProducts = products.ToPagedList(page, 4);
            return View("Index", pagedProducts);
        }








    }
}

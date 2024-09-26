using Microsoft.AspNetCore.Mvc;
using MyShop.Entities.Repositories;
using MyShop.Utilities;
using System.Security.Claims;

namespace MyShop.Web.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionKey) != null)
                {
                    HttpContext.Session.SetInt32(SD.SessionKey,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value)
                    .ToList().Count()
                    );
                    return View(HttpContext.Session.GetInt32(SD.SessionKey));
                }
                else
                {

                    return View(HttpContext.Session.GetInt32(SD.SessionKey));
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Entities.Repositories;
using MyShop.Utilities;

namespace MyShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            ViewBag.Orders = _unitOfWork.OrderHeader.GetAll().Count();
            ViewBag.ApproveOrders = _unitOfWork.OrderHeader.GetAll(x => x.OrderStatus == SD.Approve).Count();
            ViewBag.RefundOrders = _unitOfWork.OrderHeader.GetAll(x => x.OrderStatus == SD.Refund).Count();
            ViewBag.CancelledOrders = _unitOfWork.OrderHeader.GetAll(x => x.OrderStatus == SD.Cancelled).Count();
            ViewBag.ShippedOrders = _unitOfWork.OrderHeader.GetAll(x => x.OrderStatus == SD.Shipped).Count();

            ViewBag.Porducts = _unitOfWork.Product.GetAll().Count();
            ViewBag.Users = _unitOfWork.ApplicationUser.GetAll().Count();
            ViewBag.Categories = _unitOfWork.Category.GetAll().Count();

            return View();
        }
    }
}

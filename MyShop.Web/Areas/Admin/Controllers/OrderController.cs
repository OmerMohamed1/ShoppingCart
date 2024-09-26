using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Entities.ViewModels;
using MyShop.Utilities;
using Stripe;

namespace MyShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderMV OrderMV { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetData()
        {
            IEnumerable<OrderHeader> orderHeaders;
            orderHeaders = _unitOfWork.OrderHeader.GetAll(Includeword: "ApplicationUser");
            return Json(new { data = orderHeaders });
        }

        public IActionResult Details(int orderid)
        {
            OrderMV orderMV = new OrderMV()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstorDefaulte(x => x.Id == orderid, Includeword: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderid, Includeword: "Product")
            };
            return View(orderMV);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {
            var orderFromDb = _unitOfWork.OrderHeader.GetFirstorDefaulte(x => x.Id == OrderMV.OrderHeader.Id);

            orderFromDb.Name = OrderMV.OrderHeader.Name;
            orderFromDb.PhoneNumber = OrderMV.OrderHeader.PhoneNumber;
            orderFromDb.Address = OrderMV.OrderHeader.Address;
            orderFromDb.City = OrderMV.OrderHeader.City;

            if (OrderMV.OrderHeader.Carrier != null)
            {
                orderFromDb.Carrier = OrderMV.OrderHeader.Carrier;
            }

            if (OrderMV.OrderHeader.TrackingNumber != null)
            {
                orderFromDb.TrackingNumber = OrderMV.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(orderFromDb);
            _unitOfWork.Complete();
            TempData["Update"] = "Item has Updated Successfully";
            return RedirectToAction("Details", "Order", new { orderid = orderFromDb.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProccess()
        {
            _unitOfWork.OrderHeader.UpdateOrderStatus(OrderMV.OrderHeader.Id, SD.Proccessing, null);
            _unitOfWork.Complete();

            TempData["Update"] = "Order Status has Updated Successfully";
            return RedirectToAction("Details", "Order", new { orderid = OrderMV.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartShip()
        {
            var orderFromDb = _unitOfWork.OrderHeader.GetFirstorDefaulte(x => x.Id == OrderMV.OrderHeader.Id);

            orderFromDb.TrackingNumber = OrderMV.OrderHeader.TrackingNumber;
            orderFromDb.Carrier = OrderMV.OrderHeader.Carrier;
            orderFromDb.OrderStatus = SD.Shipped;
            orderFromDb.ShippingDate = DateTime.Now;

            _unitOfWork.OrderHeader.Update(orderFromDb);
            _unitOfWork.Complete();

            TempData["Update"] = "Order Shipped has Updated Successfully";
            return RedirectToAction("Details", "Order", new { orderid = OrderMV.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
            var orderFromDb = _unitOfWork.OrderHeader.GetFirstorDefaulte(x => x.Id == OrderMV.OrderHeader.Id);
            if (orderFromDb.PaymentStatus == SD.Approve)
            {
                var options = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderFromDb.paymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateOrderStatus(orderFromDb.Id, SD.Cancelled, SD.Refund);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateOrderStatus(orderFromDb.Id, SD.Cancelled, SD.Cancelled);
            }

            _unitOfWork.Complete();

            TempData["Update"] = "Order  has Cancelled Successfully";
            return RedirectToAction("Details", "Order", new { orderid = OrderMV.OrderHeader.Id });
        }
    }
}

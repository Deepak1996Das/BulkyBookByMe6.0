using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookweb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize]
	public class OrderController : Controller
	{
		private readonly IunitOfWork _unitofwork;
        [BindProperty]
        public OrderVM OrderVM { get;set; }
		public OrderController(IunitOfWork unitOfWork)
        {
			_unitofwork = unitOfWork;
		}
        public IActionResult Index()
		{
			return View();
		}

        //-----------------------------------------------------------
        public IActionResult Details(int OrderId)
        {
            OrderVM = new OrderVM()
            {
                orderHeader = _unitofwork.orderHeader.GetFirstOrDefault(u => u.Id == OrderId, includeProperties: "ApplicationUser"),
                orderDetails = _unitofwork.orderDetail.GetALl(u => u.Id == OrderId, includeProperties: "product")
            };
            return View(OrderVM);
        }


        //-----------------------------------------------------------

        [HttpPost]
        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetail(OrderVM orderVM)
        {
            var orderHeaderFromDb = _unitofwork.orderHeader.GetFirstOrDefault(u => u.Id == orderVM.orderHeader.Id);

            orderHeaderFromDb.Name = orderVM.orderHeader.Name;
            orderHeaderFromDb.PhoneNumber = orderVM.orderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = orderVM.orderHeader.StreetAddress;
            orderHeaderFromDb.City = orderVM.orderHeader.City;
            orderHeaderFromDb.state = orderVM.orderHeader.state;
            orderHeaderFromDb.PostalCode = orderVM.orderHeader.PostalCode;

           

            if(orderVM.orderHeader.Carrier != null)
            {
                orderHeaderFromDb.Carrier = orderVM.orderHeader.Carrier;
            }
            if (orderVM.orderHeader.TrackingNumber != null)
            {
                orderHeaderFromDb.TrackingNumber = orderVM.orderHeader.TrackingNumber;
            }
            _unitofwork.orderHeader.Update(orderHeaderFromDb);
            _unitofwork.Save();
            
            TempData["Success"] = "Order Details Upadated Successfully";
            return RedirectToAction("Details", "Order", new { OrderId = orderHeaderFromDb.Id });


        }
        //--------------------------------------------------

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]

        public IActionResult StartProcessing(OrderVM orderVM)
        {

            _unitofwork.orderHeader.UpdateStatus(orderVM.orderHeader.Id, SD.StatusInProcess);

            _unitofwork.Save();

            TempData["Success"] = "Order Status Upadated Successfully";
            return RedirectToAction("Details", "Order", new { OrderId =orderVM.orderHeader.Id });


        }
        //---------------------------------------------------------------


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]

        public IActionResult ShipOrder(OrderVM orderVM)
        {
            var orderHeader = _unitofwork.orderHeader.GetFirstOrDefault(u => u.Id == orderVM.orderHeader.Id);
            orderHeader.TrackingNumber = orderVM.orderHeader.TrackingNumber;
            orderHeader.Carrier = orderVM.orderHeader.Carrier;
            orderHeader.OderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if(orderHeader.PaymentStatus==SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }
            _unitofwork.orderHeader.Update(orderHeader);

            _unitofwork.Save();

            TempData["Success"] = "Order Shipped Successfully";
            return RedirectToAction("Details", "Order", new { OrderId = orderVM.orderHeader.Id });


        }
        //---------------------------------------------------


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]

        public IActionResult CancelOrder(OrderVM orderVM)
        {
            var orderHeader = _unitofwork.orderHeader.GetFirstOrDefault(u => u.Id == orderVM.orderHeader.Id);
            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitofwork.orderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.statusRefunded);
            }

            else 
            {
                _unitofwork.orderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitofwork.Save();

            TempData["Success"] = "Order Cancelled Successfully";
            return RedirectToAction("Details", "Order", new { OrderId = orderVM.orderHeader.Id });


        }
        //-----------------------------------------------------------


        #region API CALLS


        [HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders;
			if(User.IsInRole(SD.Role_Admin) || User.IsInRole( SD.Role_Employee))
			{
                orderHeaders = _unitofwork.orderHeader.GetALl(includeProperties: "ApplicationUser");
            }
			else
			{
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                orderHeaders = _unitofwork.orderHeader.GetALl(u=>u.ApplicationUserId==claim.Value,includeProperties: "ApplicationUser");
            }
			

            switch (status)
            {
                case "pending":
					orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OderStatus == SD.StatusInProcess);
                    break;
                case "complited":
                    orderHeaders = orderHeaders.Where(u => u.OderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
		}
		#endregion
	}
}

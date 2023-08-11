using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookweb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize]
	public class OrderController : Controller
	{
		private readonly IunitOfWork _unitofwork;

		public OrderController(IunitOfWork unitOfWork)
        {
			_unitofwork = unitOfWork;
		}
        public IActionResult Index()
		{
			return View();
		}

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

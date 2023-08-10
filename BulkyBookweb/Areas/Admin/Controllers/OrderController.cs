using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookweb.Areas.Admin.Controllers
{
	[Area("Admin")]
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
		public IActionResult GetAll()
		{
			IEnumerable<OrderHeader> orderHeaders;
			orderHeaders = _unitofwork.orderHeader.GetALl(includeProperties: "ApplicationUser");
			return Json(new { data = orderHeaders });
		}
		#endregion
	}
}

using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
//using BulkyBookweb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookweb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IunitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger,IunitOfWork unitOfWork,ApplicationDbContext db)
        {

            _logger = logger;
            _unitOfWork = unitOfWork;
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productsList=_unitOfWork.product.GetALl(includeProperties:"Category,CoverType");
            return View(productsList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Details(int productId) 
        {
            ShoppingCart cartObj = new()
            {
                Count = 1,
                ProductId=productId,
                product = _unitOfWork.product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,CoverType")
            };
            return View(cartObj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart cartFromDb = _unitOfWork.shoppingCart.GetFirstOrDefault(
                u => u.ApplicationUserId == claim.Value && u.ProductId==shoppingCart.ProductId);
            if (cartFromDb == null) 
            {
                _unitOfWork.shoppingCart.Add(shoppingCart);
            }
            else 
            {
                _unitOfWork.shoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
            }

           
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }


    }
}
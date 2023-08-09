using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookweb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {

        public readonly IunitOfWork _unitOfWork;

        public ShoppingCartVM ShoppingCartVM { get; set; }

        public int OrderTotal { get; set; }
        public CartController(IunitOfWork iunitOfWork)
        {
           
            _unitOfWork = iunitOfWork;
        }

        

        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                Listcart = _unitOfWork.shoppingCart.GetALl(u => u.ApplicationUserId == claim.Value,
                includeProperties: "product"),
                OrderHeader = new()
            };

            foreach(var cart in ShoppingCartVM.Listcart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.product.Price, cart.product.Price50, cart.product.Price100);

                ShoppingCartVM.OrderHeader.OrderTotal+=( cart.Count*cart.Price);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult plus(int cartId)
        {
            var cart=_unitOfWork.shoppingCart.GetFirstOrDefault(u=>u.Id == cartId);
            _unitOfWork.shoppingCart.IncrementCount(cart,1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult minus(int cartId)
        {
            var cart = _unitOfWork.shoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            if(cart.Count<=1)
            {
               _unitOfWork.shoppingCart.Remove(cart);
            }
            else 
            {
               _unitOfWork.shoppingCart.DecrementCount(cart, 1);
            }
            
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult remove(int cartId)
        {
            var cart = _unitOfWork.shoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            _unitOfWork.shoppingCart.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                Listcart = _unitOfWork.shoppingCart.GetALl(u => u.ApplicationUserId == claim.Value, includeProperties: "product"),
                OrderHeader=new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.applicationtUser.GetFirstOrDefault(u =>
            u.Id == claim.Value);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.state = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;


            foreach (var cart in ShoppingCartVM.Listcart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.product.Price, cart.product.Price50, cart.product.Price100);

                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Count * cart.Price);
            }



            return View(ShoppingCartVM);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
		public IActionResult SummaryPost()
		{
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);


            ShoppingCartVM.Listcart = _unitOfWork.shoppingCart.GetALl(u => u.ApplicationUserId == claim.Value, 
                includeProperties: "product");

            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OderStatus = SD.StatusPending;
            ShoppingCartVM.OrderHeader.OrderDate=System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
			


			foreach (var cart in ShoppingCartVM.Listcart)
			{
				cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.product.Price, cart.product.Price50, cart.product.Price100);

				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Count * cart.Price);
			}

            _unitOfWork.orderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

			foreach (var cart in ShoppingCartVM.Listcart)
			{
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };

                _unitOfWork.orderDetail.Add(orderDetail);
                _unitOfWork.Save();
			}
            _unitOfWork.shoppingCart.RemoveRange(ShoppingCartVM.Listcart);
            _unitOfWork.Save();
            return RedirectToAction("Index", "Home");

			
		}


		private double GetPriceBasedOnQuantity(double quantity,double price,double price50,double price100)
        {
            if(quantity<=50)
            {
                return price;
            }
            else
            {
                if(quantity<=100)
                {
                    return price50;
                }
                else
                    return price100;
            }
        }
    }
}

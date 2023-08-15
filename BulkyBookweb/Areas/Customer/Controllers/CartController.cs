using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Security.Claims;
using Session = Stripe.Checkout.Session;
using SessionService = Stripe.Checkout.SessionService;

namespace BulkyBookweb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {

        public readonly IunitOfWork _unitOfWork;
        public readonly IEmailSender _emailSender;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public int OrderTotal { get; set; }
        public CartController(IunitOfWork iunitOfWork,IEmailSender emailSender)
        {
            _emailSender = emailSender;
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
                var count = _unitOfWork.shoppingCart.GetALl(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
                HttpContext.Session.SetInt32(SD.SessionCart, count);

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

            var count = _unitOfWork.shoppingCart.GetALl(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
            HttpContext.Session.SetInt32(SD.SessionCart, count);
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

            //return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(/*ShoppingCartVM ShoppingCartVM*/)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);


            ShoppingCartVM.Listcart = _unitOfWork.shoppingCart.GetALl(u => u.ApplicationUserId == claim.Value,
                includeProperties: "product");

           
            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;



            foreach (var cart in ShoppingCartVM.Listcart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.product.Price, cart.product.Price50, cart.product.Price100);

                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Count * cart.Price);
            }
			ApplicationUser applicationUser = _unitOfWork.applicationtUser.GetFirstOrDefault(u => u.Id == claim.Value);

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
				ShoppingCartVM.OrderHeader.OderStatus = SD.StatusPending;
			}
            else 
            {
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				ShoppingCartVM.OrderHeader.OderStatus = SD.StatusApproved;
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



            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //Stripe setting
                var domain = "https://localhost:44393/";

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                {
                    "card",
                },

                    LineItems = new List<SessionLineItemOptions>(),


                    Mode = "payment",
                    SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/index",

                };
                foreach (var item in ShoppingCartVM.Listcart)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.product.Title,
                            },
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);

                }

                var service = new SessionService();
                Session session = service.Create(options);

                _unitOfWork.orderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);

            }
            else
            {
                return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
            }



            //_unitOfWork.shoppingCart.RemoveRange(ShoppingCartVM.Listcart);
            //_unitOfWork.Save();
            //return RedirectToAction("Index", "Home");

			
		}

        public IActionResult OrderConfirmation(int id )
        {
            OrderHeader orderHeader = _unitOfWork.orderHeader.GetFirstOrDefault(u => u.Id == id,includeProperties: "ApplicationUser");
            if( orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
				{
                    _unitOfWork.orderHeader.UpdateStripePaymentID(id, orderHeader.SessionId, session.PaymentIntentId);
                    _unitOfWork.orderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
					_unitOfWork.Save();
				}
			}

            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order-Bulky Book", "<p>New Order Created</p>");
			
            List<ShoppingCart> shoppingCarts = _unitOfWork.shoppingCart.GetALl(u => u.ApplicationUserId ==
            orderHeader.ApplicationUserId).ToList();
            _unitOfWork.shoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
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

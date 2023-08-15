//using BulkyBook.DataAccess.Repository.IRepository;
//using BulkyBook.Utility;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace BulkyBookweb.ViewComponents
//{
//    public class ShoppingCartViewComponent : ViewComponent
//    {
//        private readonly IunitOfWork _unitOfWork;
//        public ShoppingCartViewComponent(IunitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }

//        public async Task<IViewComponetResult> InvokeAsync()
//        {
//            var claimsIdentity=(ClaimsIdentity)User.Identity;
//            var claim=claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
//            if(claim!=null)
//            {
//                if(HttpContext.Session.GetInt32(SD.SessionCart)!=null)
//                {
//                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
//                }
//                else
//                {
//                    HttpContext.Session.SetInt32(SD.SessionCart,
//                        _unitOfWork.shoppingCart.GetALl(u => u.ApplicationUserId == claim.Value).ToList().Count);
//                    return View(HttpContext.Session.GetInt32(SD.SessionCart));


//                }
//            }
//            else
//            {
//                HttpContext.Session.Clear();
//                return View(0);
//            }
//        }
//    }
//}

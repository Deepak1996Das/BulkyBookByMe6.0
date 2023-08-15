using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace BulkyBookweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IunitOfWork _unitofwork;


        public CompanyController(IunitOfWork unitOfWork)
        {
            _unitofwork = unitOfWork;
           

        }
        public IActionResult Index()
        {
            return View();
        }

        // Get Greate and Update method for Product(basically we call this as Upsert 
        [HttpGet]
        public IActionResult Upsert(int? id)
        {

            Company company = new Company();

            if (id == null || id == 0)
            {
                
                return View(company);
            }

            else
            {
                //Update product
                company = _unitofwork.company.GetFirstOrDefault(u => u.Id == id);
                return View(company);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
               

                if(obj.Id==0)
                {
                    _unitofwork.company.Add(obj);
                    TempData["success"] = "Product Created successfully";
                }
                else
                {
                    _unitofwork.company.UpDate(obj);
                    TempData["success"] = "Product Updated successfully";

                }

                _unitofwork.Save();
               
                return RedirectToAction("Index");
            }

            return View(obj);

        }



        #region API CALLS
        
       
        [HttpGet]


        public IActionResult GetAll()
        {
            var CompanyList = _unitofwork.company.GetALl();
            return Json(new { data=CompanyList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitofwork.company.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, Message = "Error while Deleting" });
            }

          
            _unitofwork.company.Remove(obj);
            _unitofwork.Save();
            return Json(new { success = true, Message = "Delete successful" });
        }


        #endregion

    }

}

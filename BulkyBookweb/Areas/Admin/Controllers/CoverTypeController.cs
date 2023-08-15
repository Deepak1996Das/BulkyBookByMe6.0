using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IunitOfWork _unitofwork;

        public CoverTypeController(IunitOfWork unitOfWork)
        {
            _unitofwork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<CoverType> coverTypes=_unitofwork.CoverType.GetALl();
            return View(coverTypes);
        }
        // Get create method for CoverType 
        public IActionResult Create()
        {
            return View();
        }

        // Post Create method for CoverType
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType coverType)
        {
            if(ModelState.IsValid)
            {
                _unitofwork.CoverType.Add(coverType);
                _unitofwork.Save();
                TempData["success"] = "Cover Type Created successfully";

                return RedirectToAction("Index");
            }

            return View(coverType);

        }

        // Get create method for CoverType 
        public IActionResult Edit(int id)
        {
            if(id==null || id==0)
            {
                return NotFound();
            }
            CoverType coverType = _unitofwork.CoverType.GetFirstOrDefault(u => u.Id == id);

            if(coverType==null)
            {
                return NotFound();
            }
            return View(coverType);
        }

        // Post Create method for CoverType
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                _unitofwork.CoverType.Update(coverType);
                _unitofwork.Save();
                TempData["success"] = "Cover Type Updated successfully";

                return RedirectToAction("Index");
            }

            return View(coverType);

        }

        //Get delete method for Cover Type

        public IActionResult Delete(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            CoverType coverType = _unitofwork.CoverType.GetFirstOrDefault(u => u.Id == id);

            if (coverType == null)
            {
                return NotFound();
            }
            return View(coverType);
        }

        //Post Delete  method for Cover Type 
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(CoverType coverType) 
        {
            _unitofwork.CoverType.Remove(coverType);
            _unitofwork.Save();

            return RedirectToAction("Index");
        }

    }
}

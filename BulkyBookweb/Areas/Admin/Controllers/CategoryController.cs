using BulkyBook.Models;
//using BulkyBook.Data;
//using BulkyBookweb.Models;
using Microsoft.AspNetCore.Mvc;
//using BulkyBookweb.Data;
using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBookweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IunitOfWork _unitofwork;

        public CategoryController(IunitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _unitofwork.Category.GetALl();

            return View(objCategoryList);
        }

        //Get Method for create Category
        public IActionResult Create()
        {
            return View();
        }

        // Post method for create category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The DisplayOder can not exactly match the name.");
            }
            if (ModelState.IsValid)
            {
                _unitofwork.Category.Add(category);
                _unitofwork.Save();
                TempData["Success"] = "Category created successful";
                return RedirectToAction("Index");
            }
            return View(category);

        }

        //Get method  Edit action
        public IActionResult Edit(int id)
        {
            Category category = _unitofwork.Category.GetFirstOrDefault(u => u.Id == id);
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The DisplayOder can not exactly match the name.");
            }
            if (ModelState.IsValid)
            {
                _unitofwork.Category.Update(category);
                _unitofwork.Save();
                TempData["Success"] = "Category Edited successful";

                return RedirectToAction("Index");
            }
            return View(category);

        }

        //Get method for delete a category
        public IActionResult Delete(int id)
        {
            Category category = _unitofwork.Category.GetFirstOrDefault(u => u.Id == id);
            return View(category);
        }

        //Post method for Delete category 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category category)
        {

            _unitofwork.Category.Remove(category);
            _unitofwork.Save();
            TempData["Success"] = "Category Deleted successful";
            return RedirectToAction("Index");
        }

    }
}

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
    public class ProductController : Controller
    {
        private readonly IunitOfWork _unitofwork;

        private readonly IWebHostEnvironment _hostEnvironment;

       

        public ProductController(IunitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitofwork = unitOfWork;
            _hostEnvironment = hostEnvironment;

        }
        public IActionResult Index()
        {
            return View();
        }

        // Get Greate and Update method for Product(basically we call this as Upsert 
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
           

            //Product product = new Product();
            //IEnumerable<SelectListItem> categorylist = _unitofwork.Category.GetALl().Select(
            //    u => new SelectListItem
            //    {
            //        Text = u.Name,
            //        Value = u.Id.ToString()
            //    }) ;
            //IEnumerable<SelectListItem> CoverTypelist = _unitofwork.CoverType.GetALl().Select(
            //    u => new SelectListItem
            //    {
            //        Text = u.Name,
            //        Value = u.Id.ToString()
            //    });


            ProductVM productVM = new() 
            {
                Product=new(),
                CategoryList = _unitofwork.Category.GetALl().Select(
                    u=>new SelectListItem
                    {
                        Text=u.Name,
                        Value = u.Id.ToString()
                    }),
                CoverTypeList=_unitofwork.CoverType.GetALl().Select(
                    u=> new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()

                    })
                    
            };

           
            if (id==null || id==0)
            {
                //ViewBag.categorylist = categorylist;
                //ViewData["CoverType"] = CoverTypelist;
                // Create Product
                return View(productVM);
            }

            else
            {
                //Update product
                productVM.Product =_unitofwork.product.GetFirstOrDefault(u => u.Id==id);
                return View(productVM);
            }

          

        }

        // Post Create method for CoverType
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName=Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"Images\Products\");
                    var extension=Path.GetExtension(file.FileName);

                    if(obj.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath)) 
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStrams=new FileStream(Path.Combine(uploads,fileName+extension), FileMode.Create)) 
                    {
                        file.CopyTo(fileStrams);
                    }
                    obj.Product.ImageUrl=@"\Images\Products\"+fileName+extension;
                }

                if(obj.Product.Id==0)
                {
                    _unitofwork.product.Add(obj.Product);
                }
                else
                {
                    _unitofwork.product.Update(obj.Product);
                }
               
                _unitofwork.Save();
                TempData["success"] = "Product Created successfully";

                return RedirectToAction("Index");
            }

            return View(obj);

        }

        //Get delete method for Cover Type

        //public IActionResult Delete(int? id)
        //{
        //    Product product = _unitofwork.product.GetFirstOrDefault(u => u.Id == id);
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    CoverType coverType = _unitofwork.CoverType.GetFirstOrDefault(u => u.Id == product.CoverTypeId);


        //    if (coverType == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(coverType);
        //}

        //Post Delete  method for Cover Type 


        #region API CALLS
        
       
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitofwork.product.GetALl(includeProperties:"Category,CoverType");
            return Json(new { data=productList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitofwork.product.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, Message = "Error while Deleting" });
            }

            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitofwork.product.Remove(obj);
            _unitofwork.Save();
            return Json(new { success = true, Message = "Delete successful" });
        }


        #endregion

    }

}

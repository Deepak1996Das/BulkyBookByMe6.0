using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    internal class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db):base(db) 
        {
            _db = db;
        }
        public void Update(Product product)
        {
           var productFromDb=_db.products.FirstOrDefault(u=>u.Id == product.Id);

            if (productFromDb != null) 
            {
                productFromDb.ISBN = product.ISBN;
                productFromDb.Title = product.Title;
                productFromDb.Description = product.Description;
                productFromDb.Price = product.Price;
                productFromDb.CategoryId = product.CategoryId;
                productFromDb.CoverTypeId=product.CoverTypeId;
                productFromDb.Price100 = product.Price100;
                productFromDb.Price50 = product.Price50;
                productFromDb.ListPrice = product.ListPrice;
                productFromDb.Author=product.Author;

                if(product.ImageUrl != null)
                {
                    productFromDb.ImageUrl = product.ImageUrl;
                }


            }
        }
    }
}

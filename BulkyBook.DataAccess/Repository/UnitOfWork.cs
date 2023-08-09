using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IunitOfWork
    {
        private ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category=new CategoryRepository(_db);
            CoverType =new CoverTypeRepository(_db);
            product = new ProductRepository(_db);
            company = new CompanyRepository(_db);
            applicationtUser = new ApplicationtUserRepository(_db);
            shoppingCart= new ShoppingCartRepository(_db);
            orderDetail = new OrderDetailRepository(_db);
            orderHeader = new OrderHeaderRepository(_db);


        }
       
        public ICategoryRepository Category { get;private set; }

        public ICoverTypeRepository CoverType { get;private set; }

        public IProductRepository product { get; private set; }

        public ICompanyRepository company { get; private set; }

        public IShoppingCartRepository shoppingCart { get; private set; }

        public IApplicationtUserRepository applicationtUser { get; private set; }
        public IOrderHeaderRepository orderHeader { get;private set; }

        public IOrderDetailRepository orderDetail { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IunitOfWork
    {
        ICategoryRepository Category { get; }

        ICoverTypeRepository CoverType { get; }

        IProductRepository product { get; }

        ICompanyRepository company { get; }

        IShoppingCartRepository shoppingCart { get; }

        IApplicationtUserRepository applicationtUser { get; }

        IOrderDetailRepository orderDetail { get; }
        IOrderHeaderRepository orderHeader { get; }
        void Save();
    }
}

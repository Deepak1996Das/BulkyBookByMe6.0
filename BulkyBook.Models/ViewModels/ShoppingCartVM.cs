using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> Listcart { get; set; }

        public double CartTotal { get; set; }

        public OrderHeader OrderHeader { get; set; }

        public OrderDetail OrderDetail { get; set; }
    }
}

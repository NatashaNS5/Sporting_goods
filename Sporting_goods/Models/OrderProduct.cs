using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sporting_goods.Models
{
    public class OrderProduct
    {
        public int OrderID { get; set; }
        public string ProductArticleNumber { get; set; } = string.Empty;
        public Order1? Order { get; set; }
        public Product? Product { get; set; }
    }
}

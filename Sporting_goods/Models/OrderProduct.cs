using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sporting_goods.Models
{
    public class OrderProduct
    {
        [Key, Column(Order = 0)]
        public int OrderID { get; set; }

        [Key, Column(Order = 1)]
        public string ProductArticleNumber { get; set; }

        public virtual Order1 Order { get; set; }
        public virtual Product Product { get; set; }
    }
}

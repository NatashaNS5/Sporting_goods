using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sporting_goods.Models
{
    public class Order1
    {
        [Key]
        public int OrderID { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime? OrderDeliveryDate { get; set; }
        public int? OrderPickupPoint { get; set; }

        public virtual PickupPoint PickupPoint { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
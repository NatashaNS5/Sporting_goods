using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sporting_goods.Models
{
    public class PickupPoint
    {
        [Key]
        public int IDPick_upPoint { get; set; }

        public int? Index { get; set; }
        public string Address { get; set; }

        public virtual ICollection<Order1> Orders { get; set; } 
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Sporting_goods.Models
{
    public class Product
    {
        [Key]
        public string ProductArticleNumber { get; set; }

        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCategory { get; set; }
        public string ProductManufacturer { get; set; }
        public decimal ProductCost { get; set; }
        public byte ProductDiscountAmount { get; set; }
        public int ProductQuantityInStock { get; set; }
        public string ProductStatus { get; set; }
        public string ProductSupplier { get; set; }

        public string ProductPhoto { get; set; } 

        [NotMapped]
        public BitmapImage ProductPhotoImage
        {
            get
            {
                if (string.IsNullOrEmpty(ProductPhoto)) return null;

                try
                {
                    var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProductPhoto);
                    if (File.Exists(imagePath))
                    {
                        return new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при загрузке изображения: {ex.Message}");
                }

                return null;
            }
        }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
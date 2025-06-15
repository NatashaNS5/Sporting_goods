using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Sporting_goods.Models
{
    public class Product
    {
        public string ProductArticleNumber { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = string.Empty;
        public string? ProductPhoto { get; set; }
        public string ProductManufacturer { get; set; } = string.Empty;
        public decimal ProductCost { get; set; }
        public byte? ProductDiscountAmount { get; set; }
        public int ProductQuantityInStock { get; set; }
        public string ProductStatus { get; set; } = string.Empty;
        public string? ProductSupplier { get; set; }
        public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    }
    //public class Product
    //{
    //    public string ProductPhotoPath { get; set; }

    //    public BitmapImage ProductPhoto
    //    {
    //        get
    //        {
    //            if (string.IsNullOrEmpty(ProductPhotoPath)) return null;

    //            try
    //            {
    //                var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProductPhotoPath);

    //                if (File.Exists(imagePath))
    //                {
    //                    return new BitmapImage(new Uri(imagePath, UriKind.Absolute));
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                Console.WriteLine($"Ошибка при загрузке изображения: {ex.Message}");
    //            }

    //            return null;
    //        }
    //    }
    //}
}
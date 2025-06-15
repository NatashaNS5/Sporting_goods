using Sporting_goods.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sporting_goods
{
    public partial class Cart : Window
    {
        private List<Product> _cartItems;

        public Cart(List<Product> cartItems)
        {
            InitializeComponent();
            _cartItems = cartItems;
            UpdateCart();
        }

        private void UpdateCart()
        {
            DGCart.ItemsSource = null;
            DGCart.ItemsSource = _cartItems;

            TotalCostText.Text = $"Итого: {_cartItems.Sum(p => p.ProductCost):N2} ₽";
        }

        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = DGCart.SelectedItem as Product;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите продукт для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _cartItems.Remove(selectedProduct);
            UpdateCart();
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            if (!_cartItems.Any())
            {
                MessageBox.Show("Корзина пуста.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Order order = new Order(_cartItems.First());
            order.Show();
            this.Close();
        }

        private void CloseCart_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CurrentTabWarning_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь в этой вкладке.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

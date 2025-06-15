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
    public partial class Order : Window
    {
        public Product Product { get; private set; }

        public Order(Product product)
        {
            InitializeComponent();
            Product = product ?? new Product();
            DataContext = Product;
        }

        private void NavigateToLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.Show();
            Close();
        }

        private void NavigateToGoods_Click(object sender, RoutedEventArgs e)
        {
            var goods = new Goods();
            goods.Show();
            Close();
        }

        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            string selectedPickupPoint = pickupPointComboBox.SelectedItem is ComboBoxItem item1 ? item1.Content.ToString() : "не выбран";
            string selectedPaymentMethod = paymentMethodComboBox.SelectedItem is ComboBoxItem item2 ? item2.Content.ToString() : "не выбран";

            if (string.IsNullOrEmpty(textBox.Text))
            {
                MessageBox.Show("Введите номер телефона!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedPickupPoint == "не выбран" || selectedPaymentMethod == "не выбран")
            {
                MessageBox.Show("Выберите пункт выдачи и способ оплаты!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Заказ оформлен.\nПункт выдачи: {selectedPickupPoint}\nСпособ оплаты: {selectedPaymentMethod}\nНомер телефона: {textBox.Text}",
                            "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CurrentTabWarning_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь в данной вкладке.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, @"^\+?[78]\d{10}$"))
            {
                textBox.Foreground = Brushes.Red;
            }
            else
            {
                textBox.Foreground = Brushes.Black;
            }
        }

        private void PickupPointComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pickupPointComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                MessageBox.Show($"Вы выбрали пункт выдачи: {selectedItem.Content}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void PaymentMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (paymentMethodComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                MessageBox.Show($"Вы выбрали способ оплаты: {selectedItem.Content}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
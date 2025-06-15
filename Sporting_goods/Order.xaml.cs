using Sporting_goods.Data;
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
        private readonly ApplicationDbContext _context;
        public List<PickupPoint> PickupPoints { get; set; } 

        public Order(Product product)
        {
            InitializeComponent();
            Product = product ?? new Product();
            _context = new ApplicationDbContext();
            LoadPickupPoints();
            DataContext = this;
        }

        private void LoadPickupPoints()
        {
            try
            {
                PickupPoints = _context.PickupPoints.ToList(); 
                if (PickupPoints == null || !PickupPoints.Any())
                {
                    PickupPoints = new List<PickupPoint> { new PickupPoint { Address = "Нет доступных пунктов" } };
                    MessageBox.Show("В таблице PickupPoint нет данных.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    foreach (var point in PickupPoints)
                    {
                        Console.WriteLine($"Загружен пункт: Address={point.Address}, ID={point.IDPick_upPoint}, Index={point.Index}");
                    }
                    pickupPointComboBox.ItemsSource = null;
                    pickupPointComboBox.ItemsSource = PickupPoints;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пунктов выдачи: {ex.Message}\nПодробности: {ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                PickupPoints = new List<PickupPoint> { new PickupPoint { Address = "Ошибка загрузки" } };
                pickupPointComboBox.ItemsSource = PickupPoints;
            }
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
            string phoneNumber = textBox.Text.Trim();
            string selectedPickupPoint = pickupPointComboBox.SelectedItem is PickupPoint pickup ? pickup.Address : "не выбран";
            string selectedPaymentMethod = paymentMethodComboBox.SelectedItem is ComboBoxItem item2 ? item2.Content.ToString() : "не выбран";

            if (string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("Введите номер телефона!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedPickupPoint == "не выбран" || selectedPaymentMethod == "не выбран")
            {
                MessageBox.Show("Выберите пункт выдачи и способ оплаты!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newOrder = new Order1
                {
                    OrderStatus = "Новый",
                    OrderDate = DateTime.Now,
                    OrderDeliveryDate = DateTime.Now.AddDays(5), 
                    OrderPickupPoint = (pickupPointComboBox.SelectedItem as PickupPoint)?.IDPick_upPoint ?? 0 
                };

                _context.Orders.Add(newOrder);
                _context.SaveChanges();

                MessageBox.Show($"Заказ оформлен.\nПункт выдачи: {selectedPickupPoint}\nСпособ оплаты: {selectedPaymentMethod}\nНомер телефона: {phoneNumber}\nOrderID: {newOrder.OrderID}",
                                "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении заказа: {ex.Message}\nПодробности: {ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            if (pickupPointComboBox.SelectedItem is PickupPoint selectedItem)
            {
                MessageBox.Show($"Вы выбрали пункт выдачи: {selectedItem.Address}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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
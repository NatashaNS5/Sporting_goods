using Sporting_goods.Data;
using Sporting_goods.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <summary>
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly string _userFullName;
        private List<Product> _cartItems = new List<Product>();

        public ClientWindow(string userFullName)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _userFullName = userFullName;
            UserFullNameText.Text = _userFullName;
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var allProducts = _context.Products.ToList();
                DGproduct.ItemsSource = allProducts;

                var suppliers = _context.Products
                    .Select(p => p.ProductManufacturer)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();
                suppliers.Insert(0, "Все производители");
                SupplierComboBox.ItemsSource = suppliers;

                UpdateRecordCount(allProducts.Count, _context.Products.Count());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text;

            if (string.IsNullOrWhiteSpace(query) || query == "Введите запрос для поиска")
            {
                MessageBox.Show("Введите поисковый запрос.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var filteredProducts = _context.Products
                    .Where(p => p.ProductName.Contains(query) ||
                                p.ProductDescription.Contains(query) ||
                                p.ProductManufacturer.Contains(query))
                    .ToList();

                if (filteredProducts.Any())
                {
                    DGproduct.ItemsSource = filteredProducts;
                }
                else
                {
                    MessageBox.Show("Товары по заданному запросу не найдены.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    DGproduct.ItemsSource = new List<Product>();
                }

                UpdateRecordCount(filteredProducts.Count, _context.Products.Count());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "Введите запрос для поиска";
            SearchBox.Foreground = Brushes.Gray;
            LoadProducts();
        }

        private void UpdateRecordCount(int displayedCount, int totalCount)
        {
            RecordCountText.Text = $"{displayedCount} из {totalCount}";
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Введите запрос для поиска")
            {
                SearchBox.Text = string.Empty;
                SearchBox.Foreground = Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Введите запрос для поиска";
                SearchBox.Foreground = Brushes.Gray;
            }
        }

        private void ButtonOrder_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = DGproduct.SelectedItem as Product;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите продукт для заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _cartItems.Add(selectedProduct);
            MessageBox.Show($"Товар \"{selectedProduct.ProductName}\" добавлен в корзину.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NavigateToLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.Show();
            Close();
        }

        private void CurrentTabWarning_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь в этой вкладке.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FilterBySupplierButton_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите производителя из списка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string selectedSupplier = SupplierComboBox.SelectedItem.ToString();

            if (selectedSupplier == "Все производители")
            {
                LoadProducts();
                return;
            }

            try
            {
                var filteredProducts = _context.Products
                    .Where(p => p.ProductManufacturer == selectedSupplier)
                    .ToList();

                DGproduct.ItemsSource = filteredProducts;
                UpdateRecordCount(filteredProducts.Count, _context.Products.Count());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при фильтрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SortByPriceAscending_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sortedProducts = _context.Products.OrderBy(p => p.ProductCost).ToList();
                DGproduct.ItemsSource = sortedProducts;
                UpdateRecordCount(sortedProducts.Count, _context.Products.Count());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сортировке: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SortByPriceDescending_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sortedProducts = _context.Products.OrderByDescending(p => p.ProductCost).ToList();
                DGproduct.ItemsSource = sortedProducts;
                UpdateRecordCount(sortedProducts.Count, _context.Products.Count());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сортировке: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenCart_Click(object sender, RoutedEventArgs e)
        {
            var cartWindow = new Cart(_cartItems);
            cartWindow.ShowDialog();
        }
    }
}
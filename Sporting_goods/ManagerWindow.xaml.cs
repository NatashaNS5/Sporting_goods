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
    public partial class ManagerWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly string _userFullName;
        private ObservableCollection<Product> _products;

        public ManagerWindow(string userFullName)
        {
            InitializeComponent();
            _userFullName = userFullName;
            UserFullNameText.Text = _userFullName;
            _context = new ApplicationDbContext();
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var allProducts = _context.Products.ToList();
                _products = new ObservableCollection<Product>(allProducts);
                DGproduct.ItemsSource = _products;

                var suppliers = allProducts
                    .Select(p => p.ProductManufacturer)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();
                suppliers.Insert(0, "Все производители");
                SupplierComboBox.ItemsSource = suppliers;

                UpdateRecordCount(_products.Count, allProducts.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(query) || query == "Введите запрос для поиска")
            {
                MessageBox.Show("Введите поисковый запрос.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var lowerQuery = query.ToLower();
                var filteredProducts = _context.Products
                    .Where(p => p.ProductName.ToLower().Contains(lowerQuery) ||
                                p.ProductDescription.ToLower().Contains(lowerQuery) ||
                                p.ProductManufacturer.ToLower().Contains(lowerQuery))
                    .ToList();

                if (filteredProducts.Any())
                {
                    _products = new ObservableCollection<Product>(filteredProducts);
                    DGproduct.ItemsSource = _products;
                }
                else
                {
                    MessageBox.Show("Товары по заданному запросу не найдены.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    DGproduct.ItemsSource = new ObservableCollection<Product>();
                }

                UpdateRecordCount(filteredProducts.Count, _context.Products.Count());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении поиска: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "Введите запрос для поиска";
            SearchBox.Foreground = Brushes.Gray;
            LoadProducts();
        }

        private void UpdateRecordCount(int displayed, int total)
        {
            RecordCountText.Text = $" {displayed} из {total}";
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

        private void NavigateToLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Login();
            loginWindow.Show();
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
    }
}
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
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly string _userFullName;
        private ObservableCollection<Product> _products;

        public AdminWindow(string userFullName)
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

                var suppliers = allProducts
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

            var filteredProducts = _context.Products
                .Where(p => p.ProductName.Contains(query) ||
                            p.ProductDescription.Contains(query) ||
                            p.ProductManufacturer.Contains(query))
                .ToList();

            if (filteredProducts.Any())
            {
                DGproduct.ItemsSource = filteredProducts;
                UpdateRecordCount(filteredProducts.Count, _context.Products.Count());
            }
            else
            {
                MessageBox.Show("Товары по заданному запросу не найдены.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                DGproduct.ItemsSource = null;
                UpdateRecordCount(0, _context.Products.Count());
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

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = DGproduct.SelectedItem as Product;
            if (selectedProduct != null)
            {
                var editWindow = new AddOrEdit(selectedProduct);
                if (editWindow.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    LoadProducts();
                }
            }
            else
            {
                MessageBox.Show("Выберите продукт для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddOrEdit(null);
            if (addWindow.ShowDialog() == true)
            {
                _context.Products.Add(addWindow.Product);
                _context.SaveChanges();
                LoadProducts();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var productsForRemoving = DGproduct.SelectedItems.Cast<Product>().ToList();

            if (productsForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один продукт для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить следующие {productsForRemoving.Count} элемент(ов)?",
                                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Products.RemoveRange(productsForRemoving);
                    _context.SaveChanges();
                    LoadProducts();
                    MessageBox.Show("Удаление успешно завершено.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CurrentTabWarning_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь в этой вкладке.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NavigateToLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.Show();
            this.Close();
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
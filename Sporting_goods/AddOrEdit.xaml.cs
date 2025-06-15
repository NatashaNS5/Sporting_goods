using Sporting_goods.Data;
using Sporting_goods.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Sporting_goods
{
    public partial class AddOrEdit : Window
    {
        public Product Product { get; private set; }
        private readonly ApplicationDbContext _context;

        public AddOrEdit(Product product)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            Product = product ?? new Product();
            DataContext = Product;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveProductToDatabase();
                MessageBox.Show("Товар успешно сохранён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ComboBox1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private void CurrentTabWarning_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь в этой вкладке.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveProductToDatabase()
        {
            var existingProduct = _context.Products.FirstOrDefault(p => p.ProductArticleNumber == Product.ProductArticleNumber);

            if (existingProduct == null)
            {
                _context.Products.Add(Product);
            }
            else
            {
                _context.Entry(existingProduct).CurrentValues.SetValues(Product);
            }

            _context.SaveChanges();
        }

        private void ButtonSelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    if (bitmap.PixelWidth != 300 || bitmap.PixelHeight != 200)
                    {
                        MessageBox.Show("Размер изображения должен быть 300x200 пикселей.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string relativePath = Path.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, openFileDialog.FileName);
                    Product.ProductPhoto = relativePath; 

                    var imageControl = this.FindName("ImageControlName") as Image;
                    if (imageControl != null)
                    {
                        imageControl.Source = Product.ProductPhotoImage; 
                    }

                    MessageBox.Show("Изображение успешно добавлено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
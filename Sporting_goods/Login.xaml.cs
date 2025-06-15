using Sporting_goods.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sporting_goods
{
    public partial class Login : Window
    {
        private int failedAttempts = 0;
        private string generatedCaptcha;
        private bool isBlocked = false;
        private readonly ApplicationDbContext _context;

        public Login()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateWatermark();
        }

        private void UpdateWatermark()
        {
            watermark.Visibility = string.IsNullOrWhiteSpace(textBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TextBox1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdateWatermark1();
        }

        private void UpdateWatermark1()
        {
            watermark1.Visibility = string.IsNullOrWhiteSpace(textBox1.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (isBlocked)
            {
                MessageBox.Show("Попробуйте снова через 10 секунд.");
                return;
            }

            string login = textBox.Text.Trim();
            string password = textBox1.Password.Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            if (CaptchaPanel.Visibility == Visibility.Visible)
            {
                if (CaptchaTextBox.Text != generatedCaptcha)
                {
                    MessageBox.Show("CAPTCHA введена неверно.");
                    return;
                }
            }

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserLogin == login && u.UserPassword == password);

                if (user != null)
                {
                    MessageBox.Show($"Добро пожаловать, {user.UserSurname} {user.UserName} {user.UserPatronymic}!");

                    string fullName = $"{user.UserSurname} {user.UserName} {user.UserPatronymic}";
                    int roleId = user.UserRole ?? 0;

                    switch (roleId)
                    {
                        case 1:
                            var adminWindow = new AdminWindow(fullName);
                            adminWindow.Show();
                            break;
                        case 2:
                            var managerWindow = new ManagerWindow(fullName);
                            managerWindow.Show();
                            break;
                        case 3:
                            var clientWindow = new ClientWindow(fullName);
                            clientWindow.Show();
                            break;
                    }

                    this.Close();
                }
                else
                {
                    HandleFailedLogin();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}\nПодробности: {ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HandleFailedLogin()
        {
            failedAttempts++;

            if (failedAttempts == 1)
            {
                MessageBox.Show("Неверный логин или пароль. Попробуйте снова.");
            }
            else if (failedAttempts == 2)
            {
                GenerateCaptcha();
                CaptchaPanel.Visibility = Visibility.Visible;
                MessageBox.Show("Неверный логин или пароль. Теперь необходимо пройти проверку CAPTCHA.");
            }
            else if (failedAttempts > 2)
            {
                BlockLogin();
            }
        }

        private void GenerateCaptcha()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            generatedCaptcha = new string(Enumerable.Range(0, 4).Select(x => chars[random.Next(chars.Length)]).ToArray());

            var drawingVisual = new DrawingVisual();
            using (var context = drawingVisual.RenderOpen())
            {
                context.DrawRectangle(Brushes.White, null, new Rect(0, 0, CaptchaCanvas.Width, CaptchaCanvas.Height));

                var font = new Typeface("Comic Sans MS");
                for (int i = 0; i < generatedCaptcha.Length; i++)
                {
                    double xOffset = i * 20 + random.Next(-5, 5);
                    double yOffset = random.Next(0, 15);
                    context.DrawText(
                        new FormattedText(
                            generatedCaptcha[i].ToString(),
                            System.Globalization.CultureInfo.InvariantCulture,
                            FlowDirection.LeftToRight,
                            font,
                            20,
                            Brushes.Black,
                            VisualTreeHelper.GetDpi(this).PixelsPerDip),
                        new Point(xOffset, yOffset));
                }

                for (int i = 0; i < 5; i++)
                {
                    context.DrawLine(new Pen(Brushes.Gray, 1), new Point(random.Next(0, 200), random.Next(0, 50)), new Point(random.Next(0, 200), random.Next(0, 50)));
                }
            }

            var renderTarget = new RenderTargetBitmap((int)CaptchaCanvas.Width, (int)CaptchaCanvas.Height, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(drawingVisual);
            CaptchaCanvas.Background = new ImageBrush(renderTarget);
        }

        private async void BlockLogin()
        {
            isBlocked = true;
            MessageBox.Show("Слишком много неудачных попыток. Попробуйте снова через 10 секунд.");
            await Task.Delay(10000);
            isBlocked = false;
            failedAttempts = 0;
            CaptchaPanel.Visibility = Visibility.Collapsed;
        }

        private void CaptchaRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
        }

        private void NavigateToGoods_Click(object sender, RoutedEventArgs e)
        {
            var goods = new Goods();
            goods.Show();
            Close();
        }
    }
}
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;

using System.Text;
namespace LynxUI_Main.ViewLogin
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer introTimer;
        private int currentIndex = 0;

        private readonly List<string> imageFiles = new()
        {
            "login.png",
            "secure.png",
            "fen.png",
            "smart.png"
        };

        private readonly List<BitmapImage> introImages = new();

        public MainWindow()
        {
            InitializeComponent();

            PreloadImages();

            if (introImages.Count > 0)
            {
                introTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(3)
                };
                introTimer.Tick += IntroTimer_Tick;
                introTimer.Start();

                ShowImage(currentIndex);
            }
            else
            {
                MessageBox.Show("Picture not found to load.");
            }
        }

        private void PreloadImages()
        {
            foreach (var fileName in imageFiles)
            {
                try
                {
                    string uri = $"pack://application:,,,/ViewLogin/Img/{fileName}";
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(uri, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    introImages.Add(bitmap);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Can't load picture {fileName}: {ex.Message}");
                }
            }
        }

        private void IntroTimer_Tick(object sender, EventArgs e)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(400));
            fadeOut.Completed += (s, _) =>
            {
                currentIndex = (currentIndex + 1) % introImages.Count;
                ShowImage(currentIndex);

                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400));
                IntroImage.BeginAnimation(OpacityProperty, fadeIn);
            };

            IntroImage.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void ShowImage(int index)
        {
            try
            {
                IntroImage.Source = introImages[index];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't load picture: " + ex.Message);
            }
        }

        private bool isPasswordVisible = false;

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                VisiblePasswordBox.Text = PasswordBox_Login.Password;
                PasswordBox_Login.Visibility = Visibility.Collapsed;
                VisiblePasswordBox.Visibility = Visibility.Visible;
                try
                {
                    EyeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/ViewLogin/Img/o_eye.png", UriKind.Absolute));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Can't load o_eye.png: {ex.Message}");
                }
            }
            else
            {
                PasswordBox_Login.Password = VisiblePasswordBox.Text;
                PasswordBox_Login.Visibility = Visibility.Visible;
                VisiblePasswordBox.Visibility = Visibility.Collapsed;
                try
                {
                    EyeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/ViewLogin/Img/123.png", UriKind.Absolute));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Can't load 123.png: {ex.Message}");
                }
            }
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            this.Hide();  // Ẩn form đăng nhập
            var forgotPasswordWindow = new ForgotPasswordWindow();
            forgotPasswordWindow.Show();
            forgotPasswordWindow.Closed += (s, args) => this.Show();
        }

        // HAM PHAN DANG KI
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            return ulong.TryParse(phone, out _) && phone.Length >= 10 && phone.Length <= 11;
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }


        // PHAN DANG KI
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameBox.Text.Trim();
            string username = UsernameBox.Text.Trim();
            string phone = PhoneBox.Text.Trim();
            string email = EmailBox_Register.Text.Trim();
            string password = PasswordBox_Register.Password.Trim();
            string confirmPassword = ConfirmPasswordBox.Password.Trim();
            DateTime? birthday = BirthdayPicker.SelectedDate;

            if (string.IsNullOrEmpty(fullName) ||
                string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword) ||
                !birthday.HasValue)
            {
                MessageBox.Show("Please fill in all the fields!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidPhone(phone))
            {
                MessageBox.Show("Invalid phone number. It must be numeric and 10–11 digits.");
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Invalid email format.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string hashedPassword = HashPassword(password);

            string connectionString = "Server=TRUNGPC;Database=UserProfileDB;Trusted_Connection=True;TrustServerCertificate=True;"; // sửa lại cho đúng

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if username already exists
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        int exists = (int)checkCmd.ExecuteScalar();
                        if (exists > 0)
                        {
                            MessageBox.Show("Username already exists. Please choose another one.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    // Insert user into database
                    string insertQuery = @"INSERT INTO User1s (FullName, Username, Phone, Email, Password, Birthday)
                                   VALUES (@FullName, @Username, @Phone, @Email, @Password, @Birthday)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@Birthday", birthday.Value);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("✅ Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            BackToLogin_MouseDown(null, null);
                        }
                        else
                        {
                            MessageBox.Show("❌ Registration failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // PHAN DANG NHAP
        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox_Login.Text.Trim();
            string password = PasswordBox_Login.Password.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string hashedPassword = HashPassword(password);

            string connectionString = "Server=TRUNGPC;Database=UserProfileDB;Trusted_Connection=True;TrustServerCertificate=True;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM User1s WHERE Email = @Email AND Password = @Password";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);

                        int match = (int)cmd.ExecuteScalar();
                        if (match > 0)
                        {
                            MessageBox.Show("✅ Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Mở MainWindow
                            var mainWindow = new LynxUI_Main.MainWindow();
                            Application.Current.MainWindow = mainWindow;
                            mainWindow.Show();

                            // Đóng LoginWindow
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("❌ Invalid email or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }








        private readonly Duration animationDuration = new Duration(TimeSpan.FromMilliseconds(450));
        private readonly IEasingFunction easingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };

        private void SignUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegisterForm.Visibility = Visibility.Visible;
            RegisterTransform.X = FormContainer.ActualWidth;
            RegisterForm.Opacity = 0;

            // Slide LoginForm out
            var loginSlideOut = new DoubleAnimation(0, -FormContainer.ActualWidth, animationDuration) { EasingFunction = easingFunction };
            LoginTransform.BeginAnimation(TranslateTransform.XProperty, loginSlideOut);

            // Fade LoginForm out
            var loginFadeOut = new DoubleAnimation(1, 0, animationDuration) { EasingFunction = easingFunction };
            LoginForm.BeginAnimation(OpacityProperty, loginFadeOut);

            // Slide RegisterForm in
            var registerSlideIn = new DoubleAnimation(FormContainer.ActualWidth, 0, animationDuration) { EasingFunction = easingFunction };
            RegisterTransform.BeginAnimation(TranslateTransform.XProperty, registerSlideIn);

            // Fade RegisterForm in
            var registerFadeIn = new DoubleAnimation(0, 1, animationDuration) { EasingFunction = easingFunction };
            RegisterForm.BeginAnimation(OpacityProperty, registerFadeIn);

            loginSlideOut.Completed += (s, _) =>
            {
                LoginForm.Visibility = Visibility.Collapsed;
            };
        }

        private void BackToLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginForm.Visibility = Visibility.Visible;
            LoginTransform.X = -FormContainer.ActualWidth;
            LoginForm.Opacity = 0;

            // Slide RegisterForm out
            var registerSlideOut = new DoubleAnimation(0, FormContainer.ActualWidth, animationDuration) { EasingFunction = easingFunction };
            RegisterTransform.BeginAnimation(TranslateTransform.XProperty, registerSlideOut);

            // Fade RegisterForm out
            var registerFadeOut = new DoubleAnimation(1, 0, animationDuration) { EasingFunction = easingFunction };
            RegisterForm.BeginAnimation(OpacityProperty, registerFadeOut);

            // Slide LoginForm in
            var loginSlideIn = new DoubleAnimation(-FormContainer.ActualWidth, 0, animationDuration) { EasingFunction = easingFunction };
            LoginTransform.BeginAnimation(TranslateTransform.XProperty, loginSlideIn);

            // Fade LoginForm in
            var loginFadeIn = new DoubleAnimation(0, 1, animationDuration) { EasingFunction = easingFunction };
            LoginForm.BeginAnimation(OpacityProperty, loginFadeIn);

            registerSlideOut.Completed += (s, _) =>
            {
                RegisterForm.Visibility = Visibility.Collapsed;
            };
        }
    }
}

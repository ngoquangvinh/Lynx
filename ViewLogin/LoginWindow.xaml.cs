using LynxUI_Main.Models;
using LynxUI_Main.Services;
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
namespace LynxUI_Main.ViewLogin
{
    public partial class LoginWindow : Window
    {
        private DispatcherTimer introTimer;
        private int currentIndex = 0;
        public int? LoggedInUserId { get; private set; }

        private readonly List<string> imageFiles = new()
        {
            "login.png",
            "secure.png",
            "fen.png",
            "smart.png"
        };

        private readonly List<BitmapImage> introImages = new();

        public LoginWindow()
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


        // SIGNUP
        private async void Register_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Invalid phone number. It must be numeric and 10 digits.");
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

            var Rmodel = new RegisterModel
            {
                UserName = username,
                Password = password,
                FullName = fullName,
                Email = email,
                Phone = phone,
                AvatarUrl = "/Assets/avatar_default.png", // (lấy từ UI nếu có)
                Birthday = birthday
            };

            try
            {
                await ApiService.RegisterAsync(Rmodel);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            var Lmodel = new LoginModel
            {
                UsernameOrEmail = email,
                Password = password
            };

            try
            {
                var loginResult = await ApiService.LoginAsync(Lmodel);
                var api = new ApiService();
                var user = await api.GetUserByIdAsync(loginResult.UserId);
                if (loginResult != null && loginResult.Success)
                {
                    LoggedInUserId = loginResult.UserId;
                    // Open MainWindow and close LoginWindow  
                    var mainWindow = new LynxUI_Main.MainWindow(LoggedInUserId ?? 0, user.DisplayName);
                    Application.Current.MainWindow = mainWindow;
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Login failed. Please check your credentials.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // LOGIN
        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string usernameOrEmail = EmailBox_Login.Text.Trim();
            string password = PasswordBox_Login.Password.Trim();

            if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username/email and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var model = new LoginModel
            {
                UsernameOrEmail = usernameOrEmail,
                Password = password
            };

            try
            {
                var loginResult = await ApiService.LoginAsync(model);
                var api = new ApiService();
                var user = await api.GetUserByIdAsync(loginResult.UserId);

                if (loginResult != null && loginResult.Success)
                {
                    LoggedInUserId = loginResult.UserId;
                    // Open MainWindow and close LoginWindow  
                    var mainWindow = new LynxUI_Main.MainWindow(LoggedInUserId ?? 0, user.DisplayName);
                    Application.Current.MainWindow = mainWindow;
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Login failed. Please check your credentials.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

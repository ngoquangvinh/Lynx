using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Login1
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
                    string uri = $"pack://application:,,,/Views/Img/{fileName}";
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
                    EyeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Views/Img/o_eye.png", UriKind.Absolute));
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
                    EyeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Views/Img/123.png", UriKind.Absolute));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Can't load 123.png: {ex.Message}");
                }
            }
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            new ForgotPasswordWindow().Show();
        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // Lấy dữ liệu từ form
            string fullName = FullNameBox.Text.Trim();
            string username = UsernameBox.Text.Trim();
            string phone = PhoneBox.Text.Trim();
            string email = EmailBox_Register.Text.Trim();
            string password = PasswordBox_Register.Password.Trim();
            string confirmPassword = ConfirmPasswordBox.Password.Trim();
            DateTime? birthday = BirthdayPicker.SelectedDate;

            // Kiểm tra dữ liệu nhập vào
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

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            BackToLogin_MouseDown(null, null);
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

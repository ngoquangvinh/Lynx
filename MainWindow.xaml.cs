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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;



namespace Login
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer introTimer;
        private int currentIndex = 0;

        private readonly string[] introImages = new[]
        {
            "img/login.png",
            "img/secure.png",
            "img/fen.png",
            "img/smart.png"
        };

        public MainWindow()
        {
            InitializeComponent();

            introTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            introTimer.Tick += IntroTimer_Tick;
            introTimer.Start();

            ShowImage(currentIndex);
        }

        private void IntroTimer_Tick(object sender, EventArgs e)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(400));
            fadeOut.Completed += (s, _) =>
            {
                currentIndex = (currentIndex + 1) % introImages.Length;
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
                string path = $"pack://application:,,,/{introImages[index]}";
                IntroImage.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không load được ảnh: " + ex.Message);
            }
        }
        private void SignUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var registerWindow = new RegisterWindow(); 
            registerWindow.Show();
        }
        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            new ForgotPasswordWindow().Show();
        }
        private bool isPasswordVisible = false;

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                VisiblePasswordBox.Text = PasswordBox.Password;
                PasswordBox.Visibility = Visibility.Collapsed;
                VisiblePasswordBox.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordBox.Password = VisiblePasswordBox.Text;
                PasswordBox.Visibility = Visibility.Visible;
                VisiblePasswordBox.Visibility = Visibility.Collapsed;
            }
        }
    }
}

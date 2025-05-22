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


namespace LynxUI_Main.ViewLogin
{
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter your email address.");
                return;
            }

            StatusText.Text = $"Reset link sent to {email} (simulated).";
        }
        private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }

}
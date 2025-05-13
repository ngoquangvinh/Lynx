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
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Net.Mail;

namespace Login1
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }
        private static bool IsValidEmail(string email)
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
        private static bool IsValidPhone(string phone)
        {
            return ulong.TryParse(phone, out _) && phone.Length >= 10 && phone.Length <= 11;
        }

        private void Register_Click1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(">> BẮT ĐẦU HÀM REGISTER_CLICK <<");
            string fullname = FullNameBox.Text;
            string username = UsernameBox.Text;
            string phone = PhoneBox.Text;
            string email = EmailBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            DateTime? birthday = BirthdayPicker.SelectedDate;

            if (string.IsNullOrWhiteSpace(fullname) || string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword) ||
                birthday == null)
            {
                MessageBox.Show("❌ Please fill in all fields.");
                return;
            }

            // ✅ Kiểm tra số điện thoại: chỉ số, dài 10–11
            if (!IsValidPhone(phone))
            {
                MessageBox.Show("❌ Phone number must be numeric and 10–11 digits.");
                return; // Thêm câu lệnh return này
            }

            // ✅ Kiểm tra email bằng MailAddress
            if (!IsValidEmail(email))
            {
                MessageBox.Show("❌ Invalid email format.");
                return; // Thêm câu lệnh return này
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("❌ Passwords do not match.");
                return;
            }

            
            this.Close();
        }



    }
}

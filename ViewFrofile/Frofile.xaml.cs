using LynxUI_Main.Models;
using LynxUI_Main.Services;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Diagnostics;

namespace LynxUI_Main.ViewFrofile
{
    public partial class Frofile : Window
    {
        private readonly ApiService _apiService = new ApiService();

        public Frofile()
        {
            InitializeComponent();
            LoadProfileAsync();
        }

        private async void LoadProfileAsync()
        {
            try
            {
                // Lấy userId từ token
                var userId = await _apiService.GetCurrentUserIdAsync();
                if (userId == null)
                {
                    MessageBox.Show("Không xác định được người dùng.");
                    return;
                }

                // Gọi API để lấy thông tin người dùng
                var user = await _apiService.GetUserByIdAsync(userId.Value);
                if (user == null)
                {
                    MessageBox.Show("Không thể tải thông tin người dùng.");
                    return;
                }

                // Hiển thị thông tin lên UI
                DisplayNameTextBlock.Text = user.FullName;
                UsernameTextBlock.Text =  user.DisplayName;
                EmailTextBlock.Text = user.Email;
                PhoneTextBlock.Text = user.PhoneNumber;
                BirthdayTextBlock.Text = user.Birthday.HasValue
    ? user.Birthday.Value.ToString("dd/MM/yyyy")
    : "Không rõ";


                // Load avatar
                if (!string.IsNullOrEmpty(user.AvatarUrl))
                {
                    try
                    {
                        var avatarUri = new Uri(user.AvatarUrl, UriKind.Absolute);
                        Myellipse.Fill = new ImageBrush(new BitmapImage(avatarUri));
                    }
                    catch
                    {
                        SetDefaultAvatar();
                    }
                }
                else
                {
                    SetDefaultAvatar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thông tin: " + ex.Message);
            }
        }

        private void SetDefaultAvatar()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/Assets/avatar_default.png", UriKind.Absolute);
                Myellipse.Fill = new ImageBrush(new BitmapImage(uri));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể load avatar mặc định: " + ex.Message);
            }
        }

        private async void CameraButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng cập nhật thông tin chưa được tích hợp.");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng cập nhật thông tin chưa được tích hợp.");
        }
    }
}

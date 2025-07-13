using LynxUI_Main.Models;
using LynxUI_Main.Services;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;

namespace LynxUI_Main.ViewProfile
{
    public partial class Profile : Window
    {
        private readonly ApiService _apiService = new ApiService();
        private UserItem? _currentUser;
        private string? _uploadedAvatarUrl = null;

        public Profile()
        {
            InitializeComponent();
            LoadProfileAsync();
        }

        private async void LoadProfileAsync()
        {
            try
            {
                var userId = await _apiService.GetCurrentUserIdAsync();

                if (userId == null)
                {
                    MessageBox.Show("Không xác định được người dùng.");
                    return;
                }

                var user = await _apiService.GetUserByIdAsync(userId.Value);
                if (user == null)
                {
                    MessageBox.Show("Không thể tải thông tin người dùng.");
                    return;
                }

                DisplayNameTextBlock.Text = user.FullName;
                UsernameTextBlock.Text = user.DisplayName;
                UserIdTextBlock.Text = $"ID: {user.Id}";

                EmailTextBlock.Text = user.Email;
                PhoneTextBlock.Text = user.PhoneNumber;
                BirthdayTextBlock.Text = user.Birthday.HasValue
                    ? user.Birthday.Value.ToString("dd/MM/yyyy")
                    : "Không rõ";

                _currentUser = user;

                ImageSource image;

                if (!string.IsNullOrEmpty(user.AvatarUrl))
                {
                    string fullUrl = user.AvatarUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                        ? user.AvatarUrl
                        : $"http://203.162.54.169:2090{user.AvatarUrl}";

                    image = new BitmapImage(new Uri(fullUrl, UriKind.Absolute));
                    Myellipse.Fill = new ImageBrush(image);

                    File.WriteAllText("avatar.cache", user.AvatarUrl); // ✅ Ghi lại
                }
                else if (File.Exists("avatar.cache"))
                {
                    string cachedUrl = File.ReadAllText("avatar.cache");
                    if (!string.IsNullOrEmpty(cachedUrl))
                    {
                        string fullUrl = cachedUrl.StartsWith("http")
                            ? cachedUrl
                            : $"http://203.162.54.169:2090{cachedUrl}";

                        image = new BitmapImage(new Uri(fullUrl, UriKind.Absolute));
                        Myellipse.Fill = new ImageBrush(image);
                    }
                    else
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
                var bitmap = new BitmapImage(uri);
                Myellipse.Fill = new ImageBrush(bitmap);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể load avatar mặc định: " + ex.Message);
            }
        }

        private async void CameraButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Không tìm thấy thông tin người dùng.");
                return;
            }

            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                var filePath = dialog.FileName;
                var fileUrl = await _apiService.UploadFileAsync(filePath, "avatar", _currentUser.Id);

                if (!string.IsNullOrEmpty(fileUrl))
                {
                    _uploadedAvatarUrl = fileUrl;

                    // Hiển thị ảnh ngay
                    Myellipse.Fill = new ImageBrush(new BitmapImage(new Uri(filePath)));

                    // Cập nhật thông tin người dùng
                    var dto = new UpdateUserDto
                    {
                        UserId = _currentUser.Id,
                        UserName = _currentUser.DisplayName,
                        FullName = _currentUser.FullName,
                        Email = _currentUser.Email,
                        Phone = _currentUser.PhoneNumber,
                        Birthday = _currentUser.Birthday,
                        AvatarUrl = _uploadedAvatarUrl
                    };

                    var success = await _apiService.UpdateUserAsync(dto.UserId, dto);
                    if (success)
                    {
                        MessageBox.Show("Cập nhật ảnh đại diện thành công!");
                        File.WriteAllText("avatar.cache", _uploadedAvatarUrl); // ✅ Lưu URL ảnh avatar lại

                        var mainWindow = Application.Current.MainWindow as MainWindow;
                        mainWindow?.UpdateProfileImage(_uploadedAvatarUrl);
                        LoadProfileAsync();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật ảnh thất bại!");
                    }
                }
                else
                {
                    MessageBox.Show("Upload ảnh thất bại.");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser != null)
            {
                this.Hide();
                var editWindow = new EditProfile(_currentUser, _currentUser.Id);
                var result = editWindow.ShowDialog();

                if (result == true)
                {
                    LoadProfileAsync();
                }
            }
        }
    }
}

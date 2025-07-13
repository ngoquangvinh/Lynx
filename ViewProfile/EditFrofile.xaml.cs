using LynxUI_Main.Models;
using LynxUI_Main.Services;
using System;
using System.Windows;

namespace LynxUI_Main.ViewProfile
{
    public partial class EditProfile : Window
    {
        private readonly ApiService _apiService = new ApiService();
        private readonly int _userId;
        private readonly string _avatarUrl;
        private readonly string _username;
        public EditProfile(UserItem user, int userId)
        {
            InitializeComponent();

            _userId = userId;
            _username = user.DisplayName; // hoặc user.UserName nếu có
            _avatarUrl = string.IsNullOrEmpty(user.AvatarUrl)
         ? "/Assets/avatar_default.png"
         : user.AvatarUrl;

            FullNameBox.Text = user.FullName;
            EmailBox.Text = user.Email;
            PhoneBox.Text = user.PhoneNumber;
            if (user.Birthday.HasValue)
            {
                BirthdayPicker.SelectedDate = user.Birthday.Value;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dto = new UpdateUserDto
            {
                UserName = _username, // BẮT BUỘC
                FullName = FullNameBox.Text.Trim(),
                Email = EmailBox.Text.Trim(),
                Phone = PhoneBox.Text.Trim(), // 🔁 Đổi từ PhoneNumber → Phone
                Birthday = BirthdayPicker.SelectedDate,
                AvatarUrl = _avatarUrl
            };


            var success = await _apiService.UpdateUserAsync(_userId, dto);
            if (success)
            {
                MessageBox.Show("Cập nhật thành công!");
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại!");
            }
        }
    }
}

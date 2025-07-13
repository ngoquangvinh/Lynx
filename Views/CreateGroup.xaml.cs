using LynxUI_Main.Models;
using LynxUI_Main.Services;
using System.Windows;

namespace LynxUI_Main.Views
{
    /// <summary>
    /// Interaction logic for CreateGroup.xaml
    /// </summary>
    public partial class CreateGroup : Window
    {
        private readonly ApiService _apiService = new();
        public UserItem CurrentUser { get; }

        public CreateGroup(int currentUserId)
        {
            InitializeComponent();
            CurrentUser = new UserItem { Id = currentUserId };
            this.DataContext = this;
        }

        private async void CreateGroup_Click(object sender, RoutedEventArgs e)
        {
            var selectedUsers = UserListBox.SelectedItems.Cast<UserItem>().ToList();
            selectedUsers.Add(CurrentUser);

            if (selectedUsers.Count < 2)
            {
                MessageBox.Show("Chọn ít nhất 2 người để tạo nhóm.", "Cảnh báo");
                return;
            }

            var userIds = selectedUsers.Select(u => u.Id).ToList();
            var result = await _apiService.CreateGroupChatAsync(userIds);

            if (result != null)
            {
                MessageBox.Show("Tạo nhóm thành công!", "Thành công");
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Không thể tạo nhóm.", "Lỗi");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}
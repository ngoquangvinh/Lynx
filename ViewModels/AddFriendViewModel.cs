using LynxUI_Main.Models;
using LynxUI_Main.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace LynxUI_Main.ViewModels
{
    public class AddFriendViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService = new();

        public int CurrentUserId { get; set; }

        public ObservableCollection<UserItem> MatchingUsers { get; set; } = new();

        private string _searchTerm = "";
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (_searchTerm != value)
                {
                    _searchTerm = value;
                    OnPropertyChanged();
                    _ = SearchUserAsync();
                }
            }
        }

        private UserItem _selectedUser;
        public UserItem SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand SendRequestCommand { get; }

        public AddFriendViewModel(int userId)
        {
            CurrentUserId = userId;
            SendRequestCommand = new RelayCommand(
                async obj => await ExecuteSendRequest(),
                obj => SelectedUser != null
            );
        }

        private async Task SearchUserAsync()
        {
            MatchingUsers.Clear();

            if (string.IsNullOrWhiteSpace(SearchTerm)) return;

            string input = SearchTerm.Trim();
            if (input.StartsWith("#")) input = input[1..];
            if (int.TryParse(input, out int targetId) && targetId >= 100)
            {
                System.Diagnostics.Debug.WriteLine($"Searching for user ID: {targetId}");
                var user = await _apiService.GetUserByIdAsync(targetId);
                if (user != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[Search Result] Id = {user.Id}, DisplayName = {user.DisplayName}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[Search Result] No user found.");
                }

                if (user != null && user.Id != CurrentUserId)
                {
                    MatchingUsers.Add(user);
                    SelectedUser = user;
                }
            }
        }

        private async Task ExecuteSendRequest()
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Vui lòng chọn người dùng.", "Thông báo");
                return;
            }

            bool success = await _apiService.SendFriendRequestAsync(CurrentUserId, SelectedUser.Id);
            if (success)
            {
                MessageBox.Show($"Đã gửi lời mời kết bạn đến {SelectedUser.DisplayName} (ID #{SelectedUser.Id})", "Thành công");
            }
            else
            {
                MessageBox.Show("Không thể gửi lời mời. Có thể bạn đã gửi trước đó hoặc đã là bạn.", "Thất bại");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}

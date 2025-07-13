using LynxUI_Main.Models;
using LynxUI_Main.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace LynxUI_Main.ViewModels
{
    public class CreateGroupViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService = new();

        public int CurrentUserId { get; set; }

        public ObservableCollection<UserItem> MatchingUsers { get; set; } = new();
        public ObservableCollection<UserItem> SelectedUsers { get; set; } = new();

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged();
                _ = SearchUserAsync();
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
            }
        }

        public ICommand AddUserCommand { get; }

        public CreateGroupViewModel(int userId)
        {
            CurrentUserId = userId;
            AddUserCommand = new RelayCommand(_ => AddSelectedUser(), _ => SelectedUser != null && !SelectedUsers.Contains(SelectedUser));
        }

        private async Task SearchUserAsync()
        {
            MatchingUsers.Clear();

            if (string.IsNullOrWhiteSpace(SearchTerm)) return;

            string input = SearchTerm.Trim();
            if (input.StartsWith("#")) input = input[1..];
            if (int.TryParse(input, out int id) && id >= 100)
            {
                var user = await _apiService.GetUserByIdAsync(id);
                if (user != null && user.Id != CurrentUserId)
                {
                    MatchingUsers.Add(user);
                    SelectedUser = user;
                }
            }
        }

        private void AddSelectedUser()
        {
            if (SelectedUser != null && !SelectedUsers.Contains(SelectedUser))
                SelectedUsers.Add(SelectedUser);
        }

        public async void CreateGroup(List<UserItem> selectedUsers)
        {
            if (selectedUsers == null || selectedUsers.Count < 2)
            {
                MessageBox.Show("Cần chọn ít nhất 2 người để tạo nhóm.");
                return;
            }

            var ids = selectedUsers.Select(u => u.Id).ToList();
            ids.Add(CurrentUserId);

            var result = await _apiService.CreateGroupChatAsync(ids);
            if (result != null)
                MessageBox.Show("Tạo nhóm thành công!", "Thành công");
            else
                MessageBox.Show("Không thể tạo nhóm.", "Lỗi");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

}
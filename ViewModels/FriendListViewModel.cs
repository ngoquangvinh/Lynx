using LynxUI_Main.Models;
using LynxUI_Main.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LynxUI_Main.ViewModels
{
    public class FriendListViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService = new();
        public ObservableCollection<UserItem> Friends { get; set; } = new();

        public async Task LoadFriendsAsync(int currentUserId)
        {
            var list = await _apiService.GetFriendListAsync(currentUserId);
            Friends.Clear();
            foreach (var friend in list)
                Friends.Add(friend);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}

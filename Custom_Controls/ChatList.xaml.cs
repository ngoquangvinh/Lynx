using LynxUI_Main.Models;
using LynxUI_Main.Services;
using LynxUI_Main.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LynxUI_Main.Custom_Controls
{
    /// <summary>
    /// Interaction logic for ChatList.xaml
    /// </summary>
    public partial class ChatList : UserControl
    {
        public static readonly DependencyProperty CurrentUserIdProperty =
        DependencyProperty.Register(nameof(CurrentUserId), typeof(int), typeof(ChatList), new PropertyMetadata(0, OnCurrentUserIdChanged));

        public int CurrentUserId
        {
            get => (int)GetValue(CurrentUserIdProperty);
            set => SetValue(CurrentUserIdProperty, value);
        }

        private static void OnCurrentUserIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ChatList)d;
            int userId = (int)e.NewValue;
            control.DataContext = new ChatListViewModel(userId);
        }

        private ChatListViewModel _chatListViewModel; // Fix: Declare the missing field

        public event EventHandler<ChatListItem> ChatSelected;
        public ChatList()
        {
            InitializeComponent();
            _chatListViewModel = new ChatListViewModel(CurrentUserId); // Fix: Initialize the field
        }

        private void ChatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChatListBox.SelectedItem is ChatListItem selectedChat)
            {
                ChatSelected?.Invoke(this, selectedChat);
            }
        }
        private void ChatListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = FindVisualChild<ScrollViewer>(ChatListBox);
            if (scrollViewer != null)
            {
                // Mỗi dòng cao ~90 => 3 dòng ≈ 270
                double offsetChange = e.Delta > 0 ? -270 : 270;
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offsetChange);
                e.Handled = true;
            }
        }
        private async void FriendsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FriendListBox.SelectedItem is UserItem friend && DataContext is ChatListViewModel vm)
            {
                var apiService = new ApiService();
                var response = await apiService.CreateChatAsync(vm.CurrentUserId, friend.Id);
                if (response == null)
                {
                    MessageBox.Show("Không thể tạo hoặc lấy cuộc trò chuyện.");
                    return;
                }

                if (response.HasValue)
                {
                    int chatId = response.Value.ChatId;

                    var fullChat = await apiService.GetChatByIdAsync(chatId);
                    var chatItem = new ChatListItem
                    {
                        Id = response.Value.ChatId,
                        DisplayName = friend.DisplayName,
                        AvatarUrls = new ObservableCollection<string> { friend.AvatarUrl },
                        CurrentUserId = vm.CurrentUserId,
                        IsGroup = false,
                        LastMessage = "",
                        LastMessageTime = null,
                        IsOnline = friend.IsOnline,
                        UserIds = fullChat?.UserIds ?? new ObservableCollection<int> { vm.CurrentUserId, friend.Id }
                    };

                    if (!vm.Chats.Any(c => c.Id == chatItem.Id))
                        vm.Chats.Insert(0, chatItem);
                    vm.SelectedChat = chatItem;

                    // Gán sang ConversationViewModel
                    if (Application.Current.MainWindow?.DataContext is MainViewModel mainVM)
                    {
                        if (mainVM.ConversationVM != null)
                        {
                            mainVM.ConversationVM.ActiveChat = chatItem;
                            Debug.WriteLine($"[Info] Đã gán ActiveChat = {chatItem.DisplayName}");
                        }
                    }

                    vm.IsChatTabVisible = true;
                    vm.IsFriendTabVisible = false;
                }
            }
        }

        private async void AcceptFriend_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is UserItem user)
            {
                var vm = DataContext as ChatListViewModel;
                await vm.AcceptFriendAsync(user.Id, vm.CurrentUserId);
            }
        }

        private async void RejectFriend_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is UserItem user)
            {
                var vm = DataContext as ChatListViewModel;
                await vm.RejectFriendAsync(user.Id, vm.CurrentUserId);
            }
        }

        private T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T correctlyTyped)
                    return correctlyTyped;

                var descendent = FindVisualChild<T>(child);
                if (descendent != null)
                    return descendent;
            }
            return null;
        }
    }
}

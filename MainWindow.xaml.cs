using LynxUI_Main.Helpers;
using LynxUI_Main.Services;
using LynxUI_Main.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;

namespace LynxUI_Main
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;
        private readonly int _userId;
        private readonly SignalRService _signalRService;
        private bool isSidebarOpen = false;

        public MainWindow(int userId, string displayName)
        {
            InitializeComponent();

            _userId = userId;
            _signalRService = new SignalRService();

            _mainViewModel = new MainViewModel(userId, _signalRService, displayName);
            this.DataContext = _mainViewModel;

            ChatListControl.DataContext = _mainViewModel.ChatListVM;
            ConversationControl.DataContext = _mainViewModel.ConversationVM;

            // ✅ Load avatar từ cache hoặc API
            LoadProfileImageOnStartup();

            ConnectToSignalR();
        }

        private async void ConnectToSignalR()
        {
            //string serverUrl = "https://localhost:7031/ChatHub";
            string serverUrl = "http://203.162.54.169:2090/chatHub"; // Connect to Server
            await _signalRService.ConnectAsync(serverUrl, _userId);
        }

        public void ToggleSidebar()
        {
            double from = SidebarColumn.ActualWidth;
            double to = isSidebarOpen ? 0 : 310;

            var animation = new GridLengthAnimation
            {
                From = new GridLength(from),
                To = new GridLength(to),
                Duration = TimeSpan.FromSeconds(0.3)
            };

            SidebarColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);
            isSidebarOpen = !isSidebarOpen;
        }

        private void BtnToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            var animation = new GridLengthAnimation
            {
                From = SidebarColumn.Width,
                To = isSidebarOpen ? new GridLength(0) : new GridLength(310),
                Duration = TimeSpan.FromSeconds(0.2)
            };

            SidebarColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);
            isSidebarOpen = !isSidebarOpen;
        }

        public async void ShowFriendTab()
        {
            _mainViewModel.ChatListVM.IsChatTabVisible = false;
            _mainViewModel.ChatListVM.IsFriendTabVisible = true;
            await _mainViewModel.ChatListVM.LoadFriendsAsync(_userId);
            await _mainViewModel.ChatListVM.LoadIncomingRequestsAsync(_userId);
        }

        public void ShowChatTab()
        {
            _mainViewModel.ChatListVM.IsFriendTabVisible = false;
            _mainViewModel.ChatListVM.IsChatTabVisible = true;
            if (_mainViewModel.ChatListVM.Chats == null || !_mainViewModel.ChatListVM.Chats.Any())
            {
                _ = _mainViewModel.ChatListVM.LoadChatsAsync();
            }
        }
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ViewFrofile.Frofile();
            profileWindow.Owner = this; // Gắn cửa sổ cha (tùy chọn)
            profileWindow.ShowDialog(); // hoặc Show() nếu muốn không chặn
        }

        public void UpdateProfileImage(string avatarUrl)
        {
            try
            {
                var imageUri = avatarUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                    ? avatarUrl
                    : $"http://203.162.54.169:2090{avatarUrl}?t={DateTime.Now.Ticks}"; // tránh cache

                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(imageUri, UriKind.Absolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                MyAvatarButton.ProfileImageSource = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật avatar: " + ex.Message);
            }
        }

        private async void LoadProfileImageOnStartup()
        {
            try
            {
                string? avatarUrl = null;

                // Ưu tiên đọc từ cache nếu có
                if (File.Exists("avatar.cache"))
                {
                    avatarUrl = File.ReadAllText("avatar.cache");
                }
                else
                {
                    // Nếu không có cache thì gọi API lấy avatar từ server
                    var user = await new ApiService().GetUserByIdAsync(_userId);
                    if (user != null && !string.IsNullOrEmpty(user.AvatarUrl))
                    {
                        avatarUrl = user.AvatarUrl;
                        File.WriteAllText("avatar.cache", avatarUrl); // ✅ Cache lại
                    }
                }

                if (!string.IsNullOrEmpty(avatarUrl))
                {
                    var fullUrl = avatarUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                        ? avatarUrl
                        : $"http://203.162.54.169:2090{avatarUrl}?t={DateTime.Now.Ticks}";

                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(fullUrl, UriKind.Absolute);
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();

                    MyAvatarButton.ProfileImageSource = image;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load avatar khi khởi động: " + ex.Message);
            }
        }

    }
}

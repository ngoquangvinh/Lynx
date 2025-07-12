using LynxUI_Main.Helpers;
using LynxUI_Main.Models;
using LynxUI_Main.Services;
using LynxUI_Main.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace LynxUI_Main.ViewModels
{
    public class ChatListItem : INotifyPropertyChanged
    {
        [JsonPropertyName("chatId")]
        public int Id { get; set; }
        public string DisplayName { get; set; }
        private string _lastMessage;
        public string LastMessage
        {
            get => _lastMessage;
            set
            {
                _lastMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormattedLastMessage));
            }
        }
        public DateTime? LastMessageTime { get; set; }
        public bool IsOnline { get; set; }
        public bool IsChatSelected { get; set; }
        public bool IsGroup { get; set; }
        public bool IsDelete { get; set; }
        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormattedLastMessage));
            }
        }

        public ObservableCollection<string> AvatarUrls { get; set; } = new();
        public ObservableCollection<int> UserIds { get; set; } = new ObservableCollection<int>();

        private int _lastSenderId;
        public int LastSenderId
        {
            get => _lastSenderId;
            set
            {
                _lastSenderId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormattedLastMessage));
            }
        }
        private string _lastSenderName;
        public string LastSenderName
        {
            get => _lastSenderName;
            set
            {
                _lastSenderName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormattedLastMessage));
            }
        }
        private string _lastMessageType;
        public string LastMessageType
        {
            get => _lastMessageType;
            set
            {
                _lastMessageType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormattedLastMessage));
            }
        }
        public int CurrentUserId { get; set; }

        public ImageSource AvatarImage
        {
            get
            {
                string path = (AvatarUrls != null && AvatarUrls.Any() && !string.IsNullOrWhiteSpace(AvatarUrls[0]))
                    ? AvatarUrls[0]
                    : "Assets/avatar_default.png";

                return ImageHelper.GetAvatarImage(path);
            }
        }

        public string AvatarImageSource
        {
            get
            {
                string raw = (AvatarUrls != null && AvatarUrls.Any() && !string.IsNullOrWhiteSpace(AvatarUrls[0]))
                    ? AvatarUrls[0]
                    : "Assets/avatar_default.png";

                string fullPath = NormalizePath(raw);
                Debug.WriteLine($"[Debug] AvatarImageSource resolved to: {fullPath}");
                return fullPath;
            }
        }

        public ObservableCollection<object> AvatarSlots
        {
            get
            {
                var result = new ObservableCollection<object>();
                int count = AvatarUrls?.Count ?? 0;

                for (int i = 0; i < Math.Min(3, count); i++)
                {
                    result.Add(NormalizePath(AvatarUrls[i]));
                }

                if (count > 4)
                {
                    result.Add($"+{count - 3}");
                }
                else if (count == 4)
                {
                    result.Add(NormalizePath(AvatarUrls[3]));
                }

                return result;
            }
        }


        private string NormalizePath(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return Path.Combine(AppContext.BaseDirectory, "Assets", "avatar_default.png");

            // Nếu là absolute URL (http://, file://, etc.)
            if (Uri.IsWellFormedUriString(raw, UriKind.Absolute))
                return raw;

            // Nếu là đường dẫn bắt đầu từ /Assets hoặc Assets
            if (raw.StartsWith("/Assets/", StringComparison.OrdinalIgnoreCase) ||
                raw.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            {
                var relative = raw.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                return Path.Combine(AppContext.BaseDirectory, relative);
            }

            // Trường hợp còn lại: đường dẫn tương đối
            return Path.GetFullPath(raw);
        }

        private string GetFullPathOrUrl(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "Assets/avatar_default.png";

            return Uri.IsWellFormedUriString(raw, UriKind.Absolute)
                ? raw
                : Path.GetFullPath(raw);
        }

        public void RaiseFormattedLastMessageChanged()
        {
            OnPropertyChanged(nameof(FormattedLastMessage));
        }

        public string FormattedLastMessage
        {
            get
            {
                if (IsDelete) return "Tin nhắn đã gỡ";

                string prefix = "";

                if (IsGroup)
                {
                    if (LastSenderId == CurrentUserId)
                        prefix = "Bạn: ";
                    else
                        prefix = !string.IsNullOrWhiteSpace(LastSenderName) ? $"{LastSenderName}: " : "(Không rõ): ";
                }
                else
                {
                    if (LastSenderId == CurrentUserId)
                        prefix = "Bạn: ";
                }

                return LastMessageType switch
                {
                    "text" => prefix + LastMessage,
                    "image" => prefix + "{hình ảnh}",
                    "video" => prefix + "{video}",
                    "sticker" => prefix + "Sticker",
                    "emoji" => prefix + LastMessage,
                    "file" => prefix + (!string.IsNullOrWhiteSpace(FileName) ? FileName : "{file}"),
                    "audio" => prefix + "{âm thanh}",
                    _ => prefix + (LastMessage ?? "")
                };

            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class ChatListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ChatListItem> Chats { get; set; } = new();
        private List<ChatListItem> _allChats = new();

        public int CurrentUserId { get; set; }
        private ChatListItem _selectedChat = null!;
        public ChatListItem SelectedChat
        {
            get => _selectedChat;
            set { _selectedChat = value; OnPropertyChanged(); }
        }

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
                    FilterChats();
                }
            }
        }

        public ICommand AddChatCommand { get; }
        public ICommand AddFriendCommand { get; }
        public ICommand CreateGroupCommand { get; }
        public ICommand AcceptFriendCommand { get; }
        public ICommand RejectFriendCommand { get; }


        public ChatListViewModel(int currentUserId)
        {
            CurrentUserId = currentUserId;
            AddChatCommand = new RelayCommand(_ => ExecuteAddChat());
            AddFriendCommand = new RelayCommand(_ => ExecuteAddFriend());
            CreateGroupCommand = new RelayCommand(_ => ExecuteCreateGroup());
            AcceptFriendCommand = new RelayCommand(async user =>
            {
                if (user is UserItem userItem)
                {
                    await AcceptFriendAsync(userItem.Id, CurrentUserId);
                }
            });
            RejectFriendCommand = new RelayCommand(async user =>
            {
                if (user is UserItem userItem)
                {
                    await RejectFriendAsync(userItem.Id, CurrentUserId);
                }
            });

            _ = LoadChatsAsync();
        }

        public async Task LoadChatsAsync()
        {
            Debug.WriteLine("[LoadChatsAsync] Đang tải danh sách cuộc trò chuyện...");
            var service = new ApiService();
            var chatItems = await service.GetChatListAsync(CurrentUserId);
            var allMessages = new Dictionary<int, List<MessageItem>>();
            Debug.WriteLine($"[LoadChatsAsync] Số lượng chat lấy được từ API: {chatItems.Count}");

            foreach (var chat in chatItems)
            {
                chat.CurrentUserId = CurrentUserId;

                var messages = await service.GetMessagesAsync(chat.Id, CurrentUserId);
                messages = messages.OrderBy(m => m.TimeStamp).ToList();
                Debug.WriteLine($"[LoadChatsAsync] ChatId={chat.Id}, Số tin nhắn: {messages.Count}");
                allMessages[chat.Id] = messages;

            }

            // Đồng bộ lại LastMessage
            ChatMessageHelper.UpdateChatListItemFromMessages(chatItems, allMessages);

            _allChats = chatItems;
            FilterChats();
        }


        private void FilterChats()
        {
            Debug.WriteLine($"[FilterChats] SearchTerm: {SearchTerm}");
            Chats.Clear();
            foreach (var chat in _allChats)
            {
                if (string.IsNullOrWhiteSpace(SearchTerm) || chat.DisplayName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    Chats.Add(chat);
                    Debug.WriteLine($"[FilterChats] Đã thêm chat: {chat.DisplayName}");
                }
            }
        }

        private bool _isChatTabVisible = true;
        public bool IsChatTabVisible
        {
            get => _isChatTabVisible;
            set { _isChatTabVisible = value; OnPropertyChanged(); }
        }

        private bool _isFriendTabVisible;
        public bool IsFriendTabVisible
        {
            get => _isFriendTabVisible;
            set { _isFriendTabVisible = value; OnPropertyChanged(); }
        }

        public ObservableCollection<UserItem> Friends { get; set; } = new();
        public ObservableCollection<UserItem> IncomingRequests { get; set; } = new();

        public async Task LoadFriendsAsync(int userId)
        {
            var service = new ApiService();
            Friends.Clear();
            var friends = await service.GetFriendListAsync(userId);
            foreach (var f in friends)
                Friends.Add(f);
        }

        public async Task LoadIncomingRequestsAsync(int userId)
        {
            var service = new ApiService();
            IncomingRequests.Clear();
            var list = await service.GetIncomingRequestsAsync(userId);
            foreach (var user in list)
            {
                //System.Diagnostics.Debug.WriteLine($"[UserId: {user.Id}] DisplayName: {user.DisplayName}");
                IncomingRequests.Add(user);

            }
        }

        public async Task AcceptFriendAsync(int fromUserId, int toUserId)
        {
            var service = new ApiService();
            await service.AcceptFriendRequestAsync(fromUserId, toUserId);
            await LoadIncomingRequestsAsync(toUserId);
            await LoadFriendsAsync(toUserId);
        }

        public async Task RejectFriendAsync(int fromUserId, int toUserId)
        {
            var service = new ApiService();
            await service.RejectFriendRequestAsync(fromUserId, toUserId);
            await LoadIncomingRequestsAsync(toUserId);
        }

        public async Task<ChatListItem> OpenConversationFromFriendAsync(UserItem friend)
        {
            var existing = Chats.FirstOrDefault(c => c.Id == friend.Id && !c.IsGroup);

            if (existing != null)
            {
                SelectedChat = existing;
            }
            else
            {
                var avatar = string.IsNullOrWhiteSpace(friend.AvatarUrl)
                ? "/Assets/avatar_default.png"
                : friend.AvatarUrl;

                var newChat = new ChatListItem
                {
                    Id = friend.Id,
                    DisplayName = friend.DisplayName,
                    AvatarUrls = new ObservableCollection<string> { avatar },
                    CurrentUserId = CurrentUserId,
                    IsGroup = false,
                    LastMessage = "",
                    LastMessageTime = null,
                    IsOnline = friend.IsOnline
                };
                var chatDetail = await new ApiService().GetChatByIdAsync(newChat.Id);
                if (chatDetail != null && chatDetail.UserIds != null)
                {
                    newChat.UserIds = new ObservableCollection<int>(chatDetail.UserIds);
                }

                Chats.Insert(0, newChat);
                Debug.WriteLine("[Avatar] AvatarUrls = " + string.Join(",", newChat.AvatarUrls));
                SelectedChat = newChat;
            }

            IsChatTabVisible = true;
            IsFriendTabVisible = false;
            return SelectedChat;
        }


        private static void ExecuteAddChat()
        {
            // TODO: Hiển thị giao diện thêm bạn hoặc tạo nhóm
            System.Diagnostics.Debug.WriteLine("[+] AddChatCommand clicked.");
        }

        private void ExecuteAddFriend()
        {
            System.Diagnostics.Debug.WriteLine("[👤] AddFriendCommand clicked.");
            var window = new AddFriend(CurrentUserId); // Pass the required 'userId' parameter
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        private void ExecuteCreateGroup()
        {
            // TODO: Show Create Group dialog
            System.Diagnostics.Debug.WriteLine("[👥] CreateGroupCommand clicked.");
        }

        public async void ShowChatTab()
        {
            IsFriendTabVisible = false;
            IsChatTabVisible = true;

            if (_allChats == null || !_allChats.Any())
                await LoadChatsAsync();
        }
        public void ShowFriendTab()
        {
            IsChatTabVisible = false;
        }

        public event PropertyChangedEventHandler PropertyChanged = null!;
        private void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

}
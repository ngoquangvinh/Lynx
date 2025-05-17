using LynxUI_Main.Helpers;
using LynxUI_Main.Models;
using LynxUI_Main.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace LynxUI_Main.ViewModels
{
    public class ChatListItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public bool IsOnline { get; set; }
        public bool IsChatSelected { get; set; }
        public bool IsGroup { get; set; }
        public bool IsDelete { get; set; }

        public ObservableCollection<string> AvatarUrls { get; set; } = new();

        public int LastSenderId { get; set; }
        public string LastSenderName { get; set; }
        public string LastMessageType { get; set; } = "text";
        public int CurrentUserId { get; set; }

        public ImageSource AvatarImageSource => ImageHelper.GetAvatarImage(AvatarUrls?.FirstOrDefault());

        public ObservableCollection<object> AvatarSlots
        {
            get
            {
                var result = new ObservableCollection<object>();
                int count = AvatarUrls?.Count ?? 0;

                for (int i = 0; i < Math.Min(3, count); i++)
                {
                    result.Add(GetFullPathOrUrl(AvatarUrls[i]));
                }

                if (count > 4)
                {
                    result.Add($"+{count - 3}");
                }
                else if (count == 4)
                {
                    result.Add(GetFullPathOrUrl(AvatarUrls[3]));
                }

                return result;
            }
        }

        private string GetFullPathOrUrl(string raw)
        {
            return Uri.IsWellFormedUriString(raw, UriKind.Absolute)
                ? raw
                : Path.GetFullPath(raw);
        }

        public string FormattedLastMessage
        {
            get
            {
                if (IsDelete) return "Tin nhắn đã gỡ";

                string prefix = "";

                if (IsGroup)
                {
                    prefix = (LastSenderId == CurrentUserId) ? "Bạn: " : $"{LastSenderName}: ";
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
                    _ => prefix + LastMessage
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


        public ChatListViewModel()
        {
            PropertyChanged = null!;

            AddChatCommand = new RelayCommand(_ => ExecuteAddChat());
            AddFriendCommand = new RelayCommand(_ => ExecuteAddFriend());
            CreateGroupCommand = new RelayCommand(_ => ExecuteCreateGroup());

            _ = LoadChatsAsync();
        }

        public async Task LoadChatsAsync()
        {
            var service = new ChatService();
            var chatItems = await service.GetChatListAsync();
            var allMessages = new Dictionary<int, List<MessageItem>>();

            foreach (var chat in chatItems)
            {
                var messages = await service.GetMessagesForChatAsync(chat.Id, chat.IsGroup);
                allMessages[chat.Id] = messages;
            }

            // Đồng bộ last message info
            ChatMessageHelper.UpdateChatListItemFromMessages(chatItems, allMessages);


            _allChats = chatItems.ToList();
            FilterChats();
        }

        private void FilterChats()
        {
            Chats.Clear();
            foreach (var chat in _allChats)
            {
                if (string.IsNullOrWhiteSpace(SearchTerm) || chat.DisplayName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    Chats.Add(chat);
                }
            }
        }



        private void ExecuteAddChat()
        {
            // TODO: Hiển thị giao diện thêm bạn hoặc tạo nhóm
            System.Diagnostics.Debug.WriteLine("[+] AddChatCommand clicked.");
        }

        private void ExecuteAddFriend()
        {
            // TODO: Show Add Friend dialog
            System.Diagnostics.Debug.WriteLine("[👤] AddFriendCommand clicked.");
        }

        private void ExecuteCreateGroup()
        {
            // TODO: Show Create Group dialog
            System.Diagnostics.Debug.WriteLine("[👥] CreateGroupCommand clicked.");
        }


        public event PropertyChangedEventHandler PropertyChanged = null!;
        private void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

}
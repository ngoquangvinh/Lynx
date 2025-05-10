using LynxUI_Main.Helpers;
using LynxUI_Main.Models;
using LynxUI_Main.ViewModels;
using System.Collections.ObjectModel;
using System.IO;

namespace LynxUI_Main.Services
{
    public class ChatService
    {
        public async Task<List<ChatListItem>> GetChatListAsync()
        {
            await Task.Delay(500);
            return new List<ChatListItem>
            {
                new ChatListItem { Id = 1, DisplayName = "Nguyễn Văn A", AvatarUrls = new ObservableCollection<string> { "Assets/77.jpg" }, CurrentUserId = 2, IsOnline = true, IsGroup = false, IsDelete = false },
                new ChatListItem { Id = 2, DisplayName = "Team Dự Án Bảo Mật", AvatarUrls = new ObservableCollection<string> { "Assets/77.jpg", "Assets/77.jpg", "Assets/77.jpg" }, CurrentUserId = 2, IsOnline = false, IsGroup = true, IsDelete = false },
                new ChatListItem { Id = 3, DisplayName = "Thầy Cô CNTT", AvatarUrls = new ObservableCollection<string> { "Assets/77.jpg", "Assets/77.jpg", "Assets/77.jpg", "Assets/77.jpg" }, CurrentUserId = 2, IsOnline = true, IsGroup = true, IsDelete = false },
                new ChatListItem { Id = 4, DisplayName = "Mai Hương", AvatarUrls = new ObservableCollection<string> { "Assets/77.jpg" }, CurrentUserId = 2, IsOnline = false, IsGroup = false, IsDelete = false },
                new ChatListItem { Id = 5, DisplayName = "CLB AI UIT", AvatarUrls = new ObservableCollection<string> { "Assets/77.jpg", "Assets/77.jpg", "Assets/77.jpg" }, CurrentUserId = 2, IsOnline = true, IsGroup = true, IsDelete = false },
                new ChatListItem { Id = 6, DisplayName = "Trịnh Minh Khang", AvatarUrls = new ObservableCollection<string> { "Assets/77.jpg" }, CurrentUserId = 2, IsOnline = true, IsGroup = false, IsDelete = false }
            };
        }

        public async Task<List<MessageItem>> GetMessagesForChatAsync(int chatId, bool isGroup)
        {
            await Task.Delay(300);
            int currentUserId = 2;

            var messages = new Dictionary<int, List<MessageItem>>
            {
                [1] = new List<MessageItem>
                {
                    new MessageItem { SenderId = 1, SenderName = "Nguyễn Văn A", Message = "Bạn xem giúp hình này.", MessageStatus = "Received", TimeStamp = "08:10", CurrentUserId = currentUserId, AvatarUrl = "Assets/77.jpg", IsPicture = true, ImageSource = EnsureValidImage("Assets/_242_by_simpli58_dh4rdp4-414w-2x.jpg") },
                    new MessageItem { SenderId = currentUserId, SenderName = "Bạn", Message = "OK, nhìn rõ lắm!", MessageStatus = "Sent", TimeStamp = "08:12", CurrentUserId = currentUserId }
                },
                [2] = new List<MessageItem>
                {
                    new MessageItem { SenderId = 3, SenderName = "Lan", Message = "Mọi người check đoạn mã này giúp mình.", MessageStatus = "Received", TimeStamp = "13:02", CurrentUserId = currentUserId, AvatarUrl = "Assets/77.jpg" },
                    new MessageItem { SenderId = 4, SenderName = "Tú", Message = "Có thể cải thiện ở dòng 42.", MessageStatus = "Received", TimeStamp = "13:04", CurrentUserId = currentUserId, AvatarUrl = "Assets/77.jpg" },
                    new MessageItem { SenderId = currentUserId, Message = "Đồng ý, mình sẽ refactor đoạn đó.", MessageStatus = "Sent", TimeStamp = "13:06", CurrentUserId = currentUserId }
                },
                [3] = new List<MessageItem>
                {
                    new MessageItem { SenderId = 4, SenderName = "Thầy Minh", Message = "Gửi lại slide bài giảng tuần này", MessageStatus = "Received", TimeStamp = "14:00", CurrentUserId = currentUserId, AvatarUrl = "Assets/77.jpg" },
                    new MessageItem { SenderId = 4, MessageStatus = "Received", IsFile = true, FileName = "slide_week5.pdf", FileSize = "2.3MB", FileSource = EnsureValidFile("Assets/Hello.txt"), TimeStamp = "14:01", CurrentUserId = currentUserId, AvatarUrl = "Assets/77.jpg" },
                    new MessageItem { SenderId = currentUserId, Message = "Em đã nhận được. Cảm ơn thầy!", MessageStatus = "Sent", TimeStamp = "14:03", CurrentUserId = currentUserId }
                },
                [4] = new List<MessageItem>
                {
                    new MessageItem { SenderId = 5, SenderName = "Mai Hương", Message = "👍", MessageStatus = "Received", TimeStamp = "14:05", CurrentUserId = currentUserId, AvatarUrl = "Assets/77.jpg", IsEmoji = true, Emoji = "👍" },
                    new MessageItem { SenderId = currentUserId, Message = "Haha đúng rồi", MessageStatus = "Sent", TimeStamp = "14:07", CurrentUserId = currentUserId }
                },
                [5] = new List<MessageItem>
                {
                    new MessageItem { SenderId = 6, SenderName = "Đức", Message = "Họp chiều mai nha cả nhà.", MessageStatus = "Received", TimeStamp = "09:45", CurrentUserId = currentUserId, AvatarUrl = "Assets/77.jpg" },
                    new MessageItem { SenderId = currentUserId, Message = "OK luôn", MessageStatus = "Sent", TimeStamp = "09:46", CurrentUserId = currentUserId }
                },
                [6] = new List<MessageItem>
                {
                    new MessageItem { SenderId = 7, SenderName = "Khang", Message = "Tui gửi video nha", MessageStatus = "Received", TimeStamp = "10:01", CurrentUserId = currentUserId, AvatarUrl = "Assets/77.jpg", IsVideo = true, VideoSource = EnsureValidVideo("Assets/Doggie Corgi.mp4") },
                    new MessageItem { SenderId = currentUserId, Message = "Đẹp lắm!", MessageStatus = "Sent", TimeStamp = "10:04", CurrentUserId = currentUserId }
                }
            };

            var result = messages.ContainsKey(chatId) ? messages[chatId] : new List<MessageItem>
            {
                new MessageItem { SenderId = chatId, SenderName = "Người lạ", Message = "Xin chào!", MessageStatus = "Received", TimeStamp = "12:00", CurrentUserId = currentUserId, AvatarUrl = "Assets/77.jpg" }
            };

            foreach (var msg in result)
            {
                msg.CurrentUserId = currentUserId;
                MarkCorruptedIfMissing(msg);
            }

            return result;
        }

        public void UpdateChatListItemFromMessages(List<ChatListItem> chatList, Dictionary<int, List<MessageItem>> messageDict)
        {
            foreach (var chat in chatList)
            {
                if (messageDict.TryGetValue(chat.Id, out var messages))
                {
                    ChatMessageHelper.UpdateLastMessageSummary(chat, messages);
                }
            }
        }

        private string EnsureValidImage(string path)
        {
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp" };
            return (File.Exists(path) && validExtensions.Contains(Path.GetExtension(path).ToLowerInvariant())) ? path : "Assets/avatar_default.png";
        }

        private string EnsureValidVideo(string path)
        {
            var validExtensions = new[] { ".mp4", ".wmv", ".avi" };
            return (File.Exists(path) && validExtensions.Contains(Path.GetExtension(path).ToLowerInvariant())) ? path : string.Empty;
        }

        private string EnsureValidFile(string path)
        {
            return File.Exists(path) ? path : string.Empty;
        }

        private void MarkCorruptedIfMissing(MessageItem message)
        {
            if (message.IsPicture && !File.Exists(message.ImageSource))
            {
                message.IsCorrupted = true;
                message.Message = message.MessageStatus == "Sent" ? "Hình ảnh không gửi được" : "Hình ảnh không tìm thấy";
            }
            else if (message.IsVideo && !File.Exists(message.VideoSource))
            {
                message.IsCorrupted = true;
                message.Message = message.MessageStatus == "Sent" ? "Video không gửi được" : "Video không tìm thấy";
            }
            else if (message.IsFile && !File.Exists(message.FileSource))
            {
                message.IsCorrupted = true;
                message.Message = message.MessageStatus == "Sent" ? "Tệp không gửi được" : "Tệp không tìm thấy";
            }
        }
    }

    /*
        // Cấu hình SignalR sau này:
        private HubConnection _hubConnection;

        public async Task InitializeSignalRAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://yourserver.com/chatHub")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<List<ChatListItem>>("ReceiveChatList", (list) =>
            {
                // TODO: cập nhật ViewModel.ChatList
            });

            _hubConnection.On<MessageItem>("ReceiveMessage", (message) =>
            {
                // TODO: thêm message mới vào ViewModel.Messages
            });

            await _hubConnection.StartAsync();
            await _hubConnection.InvokeAsync("JoinChat", currentUserId);
        }
    */
}

using LynxUI_Main.Helpers;
using LynxUI_Main.Models;
using LynxUI_Main.ViewModels;
using System.IO;

namespace LynxUI_Main.Services
{
    public class ChatService
    {

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
            return (File.Exists(path) && validExtensions.Contains(Path.GetExtension(path).ToLowerInvariant()))
                ? path : Path.Combine(AppContext.BaseDirectory, "Assets", "avatar_default.png");
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

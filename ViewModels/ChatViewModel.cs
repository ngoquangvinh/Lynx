using LynxUI_Main.Models;
using LynxUI_Main.Services;
using System.Collections.ObjectModel;

namespace LynxUI_Main.ViewModels
{
    public class ChatViewModel
    {
        public ObservableCollection<MessageItem> Messages { get; } = new();

        private readonly ApiService _apiService = new ApiService();
        private readonly SignalRService _signalRService = new SignalRService();

        public int CurrentUserId { get; set; } // Lưu UserId của người dùng hiện tại

        public ChatViewModel()
        {
            // Lắng nghe tin nhắn realtime từ server (SignalR)
            _signalRService.OnReceiveMessage += OnReceiveMessage;
        }

        public async Task LoadMessagesAsync(int chatId)
        {
            var msgs = await _apiService.GetMessagesAsync(chatId, CurrentUserId);
            Messages.Clear();
            foreach (var m in msgs)
                Messages.Add(m);
        }

        public async Task ConnectSignalRAsync(string url)
        {
            await _signalRService.ConnectAsync(url, CurrentUserId);
        }

        public async Task SendMessageAsync(string text)
        {
            var message = new MessageItem
            {
                SenderId = CurrentUserId,
                Message = text,
                TimeStamp = DateTime.Now
            };
            await _signalRService.SendMessageAsync(message);
            // Add luôn vào UI để phản hồi tức thì
            Messages.Add(message);
        }

        // Khi nhận message từ server (SignalR)
        private void OnReceiveMessage(MessageItem item)
        {
            // Đảm bảo chạy trên UI thread
            App.Current.Dispatcher.Invoke(() => Messages.Add(item));
        }
    }
}

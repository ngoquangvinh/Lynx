using LynxUI_Main.Models;
using LynxUI_Main.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LynxUI_Main.ViewModels
{
    public class ConversationViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MessageItem> Messages { get; set; } = new ObservableCollection<MessageItem>();
        public ChatListItem SelectedChat { get; set; }
        public int CurrentUserId { get; set; }

        private HubConnection _hubConnection;

        private ChatListItem _activeChat;
        public ChatListItem ActiveChat
        {
            get => _activeChat;
            set
            {
                if (_activeChat != value)
                {
                    _activeChat = value;
                    OnPropertyChanged();
                    _ = LoadMessagesAsync(_activeChat);
                }
            }
        }

        private string _newMessageText;
        public string NewMessageText
        {
            get => _newMessageText;
            set { _newMessageText = value; OnPropertyChanged(); }
        }

        public ICommand SendCommand { get; }
        private SignalRService _signalRService;
        private readonly ApiService _apiService = new();
        private readonly string _currentUserDisplayName;
        private int _userId;

        public ConversationViewModel(int userId, SignalRService signalRService, string displayName)
        {
            _userId = userId;
            _signalRService = signalRService;
            _currentUserDisplayName = displayName;
            _signalRService.OnReceiveMessage += (message) =>
            {
                // Thêm tin nhắn nhận được vào Messages collection
                App.Current.Dispatcher.Invoke(() => Messages.Add(message));
            };


            // Nhấn Enter, gọi SendMessage
            SendCommand = new RelayCommand(_ => SendMessage(), _ => !string.IsNullOrWhiteSpace(NewMessageText));
        }

        public async Task LoadMessagesAsync(ChatListItem chat)
        {
            if (chat == null) return;

            var data = await _apiService.GetMessagesAsync(chat.Id, _userId);
            var sortedData = data.OrderBy(m => m.TimeStamp).ToList();
            Messages.Clear();
            foreach (var msg in sortedData)
            {
                AttachDownloadCommands(msg);
                CheckMissingContent(msg);
                Messages.Add(msg);
            }
        }

        public async void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(NewMessageText))
                return;

            if (ActiveChat == null || ActiveChat.Id == 0)
            {
                Debug.WriteLine("[Error] ActiveChat is null or invalid");
                return;
            }

            var avatar = ActiveChat.AvatarUrls?.FirstOrDefault()
                ?? Path.Combine(AppContext.BaseDirectory, "Assets", "avatar_default.png");

            // Lấy ReceiverId trong 1-1 chat
            var receiverId = ActiveChat.UserIds?.FirstOrDefault(id => id != _userId) ?? 0;

            var message = new MessageItem
            {
                ChatId = ActiveChat.Id,
                SenderId = _userId,
                SenderName = _currentUserDisplayName,
                ReceiverId = receiverId,
                Message = NewMessageText.Trim(),
                AvatarUrl = avatar,
                SenderAvatarUrl = avatar,
                MessageStatus = "Sent",
                TimeStamp = DateTime.Now,
                CurrentUserId = _userId
            };

            Debug.WriteLine($"[Debug] Sending message to ChatId = {message.ChatId}");
            AttachDownloadCommands(message);
            Messages.Add(message);
            await _signalRService.SendMessageAsync(message);
            NewMessageText = string.Empty;
        }


        private void AttachDownloadCommands(MessageItem message)
        {
            message.DownloadImageCommand = new RelayCommand(_ => DownloadWithDialog(message.ImageSource));
            message.DownloadVideoCommand = new RelayCommand(_ => DownloadWithDialog(message.VideoSource));
            message.DownloadAudioCommand = new RelayCommand(_ => DownloadWithDialog(message.AudioSource));
            message.DownloadFileCommand = new RelayCommand(_ => DownloadWithDialog(message.FileSource));
        }

        private void CheckMissingContent(MessageItem message)
        {
            if (message.MessageStatus == "Sent")
            {
                if (message.IsPicture && !File.Exists(message.ImageSource))
                    message.Message = "Không gửi được hình ảnh";
                else if (message.IsVideo && !File.Exists(message.VideoSource))
                    message.Message = "Không gửi được video";
                else if (message.IsFile && !File.Exists(message.FileSource))
                    message.Message = "Không gửi được tệp";
            }
            else if (message.MessageStatus == "Received")
            {
                if (message.IsPicture && !File.Exists(message.ImageSource))
                    message.Message = "Không tìm thấy hình ảnh";
                else if (message.IsVideo && !File.Exists(message.VideoSource))
                    message.Message = "Không tìm thấy video";
                else if (message.IsFile && !File.Exists(message.FileSource))
                    message.Message = "Không tìm thấy tệp";
            }
        }

        private void DownloadWithDialog(string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath)) return;

            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    FileName = Path.GetFileName(sourcePath),
                    Filter = "All Files|*.*"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    File.Copy(sourcePath, saveDialog.FileName, true);
                }
            }
            catch (Exception ex)
            {
                // TODO: xử lý lỗi, ví dụ hiện thông báo cho người dùng
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public async void SendImage(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
                return;

            if (ActiveChat == null || ActiveChat.Id == 0)
                return;

            string? uploadedUrl = await _apiService.UploadFileAsync(imagePath, "image", _userId);
            if (string.IsNullOrEmpty(uploadedUrl))
            {
                Debug.WriteLine("[SendImage] Upload failed.");
                return;
            }
            var fileName = Path.GetFileName(imagePath);
            var receiverId = ActiveChat.UserIds?.FirstOrDefault(id => id != _userId) ?? 0;
            var avatar = ActiveChat.AvatarUrls?.FirstOrDefault()
                ?? Path.Combine(AppContext.BaseDirectory, "Assets", "avatar_default.png");

            var message = new MessageItem
            {
                ChatId = ActiveChat.Id,
                SenderId = _userId,
                SenderName = _currentUserDisplayName,
                ReceiverId = receiverId,
                Message = fileName,
                FileName = fileName,
                ImageSource = uploadedUrl,
                FileUrl = uploadedUrl,
                IsPicture = true,
                MessageStatus = "Sent",
                TimeStamp = DateTime.Now,
                CurrentUserId = _userId,
                AvatarUrl = avatar,
                SenderAvatarUrl = avatar
            };

            AttachDownloadCommands(message);
            Messages.Add(message);
            await _signalRService.SendMessageAsync(message);
        }
        public async void SendFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return;

            if (ActiveChat == null || ActiveChat.Id == 0)
                return;

            var uploadedUrl = await _apiService.UploadFileAsync(filePath, "file", _userId);
            if (string.IsNullOrEmpty(uploadedUrl))
            {
                Debug.WriteLine("[SendFile] Upload failed.");
                return;
            }
            var fileName = Path.GetFileName(filePath);
            var receiverId = ActiveChat.UserIds?.FirstOrDefault(id => id != _userId) ?? 0;
            var avatar = ActiveChat.AvatarUrls?.FirstOrDefault()
                ?? Path.Combine(AppContext.BaseDirectory, "Assets", "avatar_default.png");

            var message = new MessageItem
            {
                ChatId = ActiveChat.Id,
                SenderId = _userId,
                SenderName = _currentUserDisplayName,
                ReceiverId = receiverId,
                Message = fileName,
                FileName = fileName,
                FileSource = uploadedUrl,
                FileUrl = uploadedUrl,
                IsFile = true,
                MessageStatus = "Sent",
                TimeStamp = DateTime.Now,
                CurrentUserId = _userId,
                AvatarUrl = avatar,
                SenderAvatarUrl = avatar
            };

            AttachDownloadCommands(message);
            Messages.Add(message);
            await _signalRService.SendMessageAsync(message);
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    /*public async Task StartListeningForMessagesAsync()
        {
            await _hubConnection.On<MessageDto>("ReceiveMessage", message =>
            {
                if (message.ChatId == ActiveChat.ChatId)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var item = MessageMapper.MapFromApiMessage(message, CurrentUserId);
                        Messages.Add(item);
                    });
                }
            });
        }*/

    /*public ChatListItem SelectedChatItem
    {
        get => _selectedChatItem;
        set
        {
            _selectedChatItem = value;
            OnPropertyChanged();
            LoadMessagesForChat(value?.Id ?? 0); // nếu cần tải tin nhắn khi chọn
        }
    }
    private ChatListItem _selectedChatItem;*/
}

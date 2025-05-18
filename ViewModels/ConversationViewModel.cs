using LynxUI_Main.Models;
using LynxUI_Main.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LynxUI_Main.ViewModels
{
    public class ConversationViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MessageItem> Messages { get; set; } = new();

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

        private readonly ChatService _chatService = new();

        public ConversationViewModel()
        {
            // Khi nút gửi được click hoặc nhấn Enter, gọi SendMessage
            SendCommand = new RelayCommand(_ => SendMessage(), _ => !string.IsNullOrWhiteSpace(NewMessageText));
        }

        public async Task LoadMessagesAsync(ChatListItem chat)
        {
            if (chat == null) return;

            var data = await _chatService.GetMessagesForChatAsync(chat.Id, chat.IsGroup);

            Messages.Clear();
            foreach (var msg in data)
            {
                AttachDownloadCommands(msg);
                CheckMissingContent(msg);
                Messages.Add(msg);
            }
        }

        public void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(NewMessageText))
                return;

            var message = new MessageItem
            {
                SenderId = ActiveChat.CurrentUserId,
                SenderName = "Bạn",
                Message = NewMessageText.Trim(),
                MessageStatus = "Sent",
                TimeStamp = DateTime.Now.ToString("HH:mm"),
                CurrentUserId = ActiveChat.CurrentUserId
            };

            AttachDownloadCommands(message);
            Messages.Add(message);
            NewMessageText = string.Empty;

            // TODO: gửi lên SignalR/server nếu cần
        }

        private void AttachDownloadCommands(MessageItem message)
        {
            message.DownloadImageCommand = new RelayCommand(_ => DownloadWithDialog(message.ImageSource));
            message.DownloadVideoCommand = new RelayCommand(_ => DownloadWithDialog(message.VideoSource));
            message.DownloadAudioCommand = new RelayCommand(_ => DownloadWithDialog(message.AudioSource));
            message.DownloadFileCommand = new RelayCommand(_ => DownloadWithDialog(message.FileSource));
        }

        public void SendImage(string imagePath)
        {
            // Thêm hình ảnh vào danh sách tin nhắn
            var message = new MessageItem
            {
                SenderId = ActiveChat?.CurrentUserId ?? 0,
                SenderName = "Bạn",
                Message = "[Hình ảnh]",
                IsPicture = true,
                ImageSource = imagePath,
                MessageStatus = "Sent",
                TimeStamp = DateTime.Now.ToString("HH:mm"),
                CurrentUserId = ActiveChat?.CurrentUserId ?? 0
            };
            AttachDownloadCommands(message);
            Messages.Add(message);
            // TODO: Gửi lên server nếu cần
        }

        public void SendFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return;

            // Danh sách các định dạng video phổ biến
            var videoExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".webm" };
            string ext = Path.GetExtension(filePath).ToLower();

            var message = new MessageItem
            {
                SenderId = ActiveChat?.CurrentUserId ?? 0,
                SenderName = "Bạn",
                Message = videoExtensions.Contains(ext) ? "[Video]" : "[Tệp]",
                IsFile = !videoExtensions.Contains(ext),
                IsVideo = videoExtensions.Contains(ext),
                FileName = Path.GetFileName(filePath),
                FileSource = filePath,
                FileSize = new FileInfo(filePath).Length.ToString(),
                VideoSource = videoExtensions.Contains(ext) ? filePath : string.Empty,
                MessageStatus = "Sent",
                TimeStamp = DateTime.Now.ToString("HH:mm"),
                CurrentUserId = ActiveChat?.CurrentUserId ?? 0
            };
            AttachDownloadCommands(message);
            Messages.Add(message);
            // TODO: Gửi lên server nếu cần
        }

        private static void CheckMissingContent(MessageItem message)
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
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

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

using LynxUI_Main.Helpers;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;

namespace LynxUI_Main.Models
{
    public class MessageItem
    {
        public int MessageId { get; set; }
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SenderName { get; set; }
        public string Message { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string MessageStatus { get; set; } // "Sent" or "Received"  

        public bool IsDelete { get; set; }

        public bool IsPicture { get; set; }
        public string ImageSource { get; set; }
        public bool IsImageError { get; set; }

        public bool IsVideo { get; set; }
        public string VideoSource { get; set; }
        public bool IsVideoError { get; set; }

        public bool IsAudio { get; set; }
        public string AudioSource { get; set; }

        public bool IsFile { get; set; }
        public string FileName { get; set; }
        public string FileSource { get; set; }
        public string FileSize { get; set; }

        public bool IsSticker { get; set; }
        public string StickerSource { get; set; }

        public bool IsEmoji { get; set; }
        public string Emoji { get; set; }
        public bool IsCorrupted { get; set; } = false;
        public int CurrentUserId { get; set; }
        public bool IsOwnMessage => SenderId == CurrentUserId;
        public string AvatarUrl { get; set; }
        public string SenderAvatarUrl { get; set; }

        public ImageSource AvatarImageSource
        {
            get
            {
                var path = string.IsNullOrWhiteSpace(SenderAvatarUrl) ? "/Assets/avatar_default.png" : SenderAvatarUrl;
                if (string.IsNullOrWhiteSpace(path))
                    path = "Assets/avatar_default.png";
                Debug.WriteLine("[MessageItem] Loading avatar from: " + path);
                return ImageHelper.GetAvatarImage(path);
            }
        }
        public bool IsTextMessage => !IsDelete && !IsPicture && !IsVideo && !IsAudio && !IsFile && !IsSticker && !IsEmoji;

        public ICommand DownloadImageCommand { get; set; }
        public ICommand DownloadVideoCommand { get; set; }
        public ICommand DownloadAudioCommand { get; set; }
        public ICommand DownloadFileCommand { get; set; }

    }
}

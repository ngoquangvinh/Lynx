using LynxUI_Main.Helpers;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LynxUI_Main.Models
{
    public class MessageItem
    {
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string Message { get; set; }
        public string TimeStamp { get; set; }
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

        public string AvatarUrl { get; set; } = string.Empty;

        public ImageSource AvatarImageSource => ImageHelper.GetAvatarImage(AvatarUrl);

        public bool IsTextMessage => !IsDelete && !IsPicture && !IsVideo && !IsAudio && !IsFile && !IsSticker && !IsEmoji;

        public ICommand DownloadImageCommand { get; set; }
        public ICommand DownloadVideoCommand { get; set; }
        public ICommand DownloadAudioCommand { get; set; }
        public ICommand DownloadFileCommand { get; set; }

        private ImageSource LoadImage(string pathOrUrl)
        {
            if (string.IsNullOrWhiteSpace(pathOrUrl)) return GetFallback();

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;

                bitmap.UriSource = Uri.IsWellFormedUriString(pathOrUrl, UriKind.Absolute)
                    ? new Uri(pathOrUrl, UriKind.Absolute)
                    : new Uri(Path.GetFullPath(pathOrUrl), UriKind.Absolute);

                bitmap.EndInit();
                return bitmap.SafeFreeze();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Avatar Load Error] {ex.Message}");
                return GetFallback();
            }
        }

        private ImageSource GetFallback()
        {
            try
            {
                var fallbackPath = Path.GetFullPath("Assets/avatar_default.png");
                var fallback = new BitmapImage(new Uri(fallbackPath, UriKind.Absolute));
                fallback.Freeze();
                return fallback;
            }
            catch
            {
                return null;
            }
        }
    }
}

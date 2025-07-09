using LynxUI_Main.Helpers;
using System.Windows.Media;

namespace LynxUI_Main.Models
{
    public class UserItemDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsOnline { get; set; }
        public ImageSource AvatarImage =>
        ImageHelper.GetAvatarImage(string.IsNullOrEmpty(AvatarUrl)
            ? "Assets/avatar_default.png"
            : AvatarUrl);
        public string AvatarImageSource
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(AvatarUrl))
                {
                    if (AvatarUrl.StartsWith("/") || AvatarUrl.StartsWith("Assets"))
                        return AvatarUrl; // sẽ được Normalize trong SafeImagePathConverter
                    return AvatarUrl;
                }

                return "/Assets/avatar_default.png";
            }
        }

    }

}

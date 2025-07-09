using LynxUI_Main.Helpers;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace LynxUI_Main.Models
{
    public class UserItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }
        [JsonPropertyName("avatarUrl")]
        public string AvatarUrl { get; set; }
        [JsonPropertyName("isOnline")]
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

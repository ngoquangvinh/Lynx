using LynxUI_Main.Models;
using System.IO;

namespace LynxUI_Main.Helpers
{
    public static class MessageMapper
    {
        public static MessageItem MapFromApiMessage(ApiMessageDto apiMsg, int currentUserId)
        {
            var item = new MessageItem
            {
                MessageId = apiMsg.MessageId,
                ChatId = apiMsg.ChatId,
                SenderId = apiMsg.SenderId,
                SenderName = apiMsg.SenderName,
                AvatarUrl = apiMsg.SenderAvatarUrl,
                TimeStamp = apiMsg.SentAt,
                CurrentUserId = currentUserId,
                IsDelete = apiMsg.IsDeleted,
                Message = apiMsg.MessageText,
                FileName = apiMsg.FileName,
                FileSize = apiMsg.FileSize,
                FileUrl = apiMsg.FileUrl,
                MessageStatus = apiMsg.Status
            };

            // Xác định loại message
            switch (apiMsg.MessageType)
            {
                case "text":
                    break;
                case "image":
                    item.IsPicture = true;
                    item.ImageSource = apiMsg.FileUrl;
                    break;
                case "video":
                    item.IsVideo = true;
                    item.VideoSource = apiMsg.FileUrl;
                    break;
                case "audio":
                    // Nếu có IsAudio, AudioSource
                    break;
                case "file":
                    item.IsFile = true;
                    item.FileSource = apiMsg.FileUrl;
                    item.FileName = apiMsg.FileName ?? Path.GetFileName(apiMsg.FileUrl);
                    break;
                case "emoji":
                    item.IsEmoji = true;
                    item.Emoji = apiMsg.Emoji;
                    break;
                case "sticker":
                    item.IsSticker = true;
                    item.StickerSource = apiMsg.Sticker;
                    break;
            }
            return item;
        }

        public static ApiMessageDto MapToApiMessage(MessageItem item)
        {
            var apiMsg = new ApiMessageDto
            {
                MessageId = item.MessageId,
                ChatId = item.ChatId,
                SenderId = item.SenderId,
                SenderName = item.SenderName,
                SenderAvatarUrl = string.IsNullOrWhiteSpace(item.AvatarUrl) || item.AvatarUrl.Contains("/Assets/")
                                ? "/Assets/avatar_default.png"
                                : item.AvatarUrl ?? "",
                IsOwnMessage = item.IsOwnMessage,
                ReceiverId = item.ReceiverId,
                MessageText = item.Message ?? "",
                FileName = item.FileName ?? "",
                FileSize = item.FileSize ?? "",
                FileUrl = "",
                Sticker = "",
                Emoji = "",
                IsDeleted = item.IsDelete,
                Status = item.MessageStatus,
                SentAt = item.TimeStamp.HasValue ? item.TimeStamp.Value : DateTime.MinValue
            };

            // MessageType
            if (item.IsPicture)
            {
                apiMsg.MessageType = "image";
                apiMsg.FileUrl = item.ImageSource ?? "";
                apiMsg.FileUrl = item.FileUrl ?? item.ImageSource ?? "";
            }
            else if (item.IsVideo)
            {
                apiMsg.MessageType = "video";
                apiMsg.FileUrl = item.VideoSource ?? "";
                apiMsg.FileUrl = item.FileUrl;
            }
            else if (item.IsFile)
            {
                apiMsg.MessageType = "file";
                apiMsg.FileUrl = item.FileSource ?? "";
                apiMsg.FileName = item.FileName ?? item.Message ?? "";
                apiMsg.FileUrl = item.FileUrl;
            }
            else if (item.IsSticker)
            {
                apiMsg.MessageType = "sticker";
                apiMsg.Sticker = item.StickerSource ?? "";
            }
            else if (item.IsEmoji)
            {
                apiMsg.MessageType = "emoji";
                apiMsg.Emoji = item.Emoji ?? "";
            }
            else // text
            {
                apiMsg.MessageType = "text";
            }
            return apiMsg;
        }
    }
}

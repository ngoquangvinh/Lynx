using LynxUI_Main.Models;
using LynxUI_Main.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace LynxUI_Main.Helpers
{
    public static class ChatMessageHelper
    {
        public static void UpdateLastMessageSummary(ChatListItem chat, List<MessageItem> messages)
        {
            if (chat == null || messages == null || messages.Count == 0)
                return;

            var lastMessage = messages.Last();

            chat.LastMessage = GetMessagePreview(lastMessage);
            chat.LastSenderId = lastMessage.SenderId;
            chat.LastSenderName = lastMessage.SenderName;
            chat.LastMessageType = GetMessageType(lastMessage);
            chat.LastMessageTime = ParseTimestampToDateTime(lastMessage.TimeStamp);
        }

        public static void UpdateChatListItemFromMessages(List<ChatListItem> chatList, Dictionary<int, List<MessageItem>> allMessages)
        {
            foreach (var chat in chatList)
            {
                if (!allMessages.TryGetValue(chat.Id, out var messages) || messages.Count == 0)
                    continue;

                var lastMsg = messages.LastOrDefault();
                if (lastMsg == null) continue;

                chat.LastMessage = lastMsg.Message ?? "";
                chat.LastMessageTime = DateTime.TryParse(lastMsg.TimeStamp, out var parsed) ? parsed : null;
                chat.LastSenderId = lastMsg.SenderId;
                chat.LastSenderName = lastMsg.SenderName;
                chat.LastMessageType = GetMessageType(lastMsg);
            }
        }
        private static string GetMessagePreview(MessageItem message)
        {
            if (message.IsDelete) return "Tin nhắn đã gỡ";
            if (message.IsPicture) return "{hình ảnh}";
            if (message.IsVideo) return "{video}";
            if (message.IsFile) return message.FileName;
            if (message.IsSticker) return "Sticker";
            if (message.IsEmoji) return message.Emoji;
            return message.Message;
        }

        private static string GetMessageType(MessageItem message)
        {
            if (message.IsPicture) return "image";
            if (message.IsVideo) return "video";
            if (message.IsFile) return "file";
            if (message.IsSticker) return "sticker";
            if (message.IsEmoji) return "emoji";
            return "text";
        }

        private static DateTime? ParseTimestampToDateTime(string timestamp)
        {
            if (DateTime.TryParse(timestamp, out DateTime result))
                return result;

            return null;
        }
    }
}

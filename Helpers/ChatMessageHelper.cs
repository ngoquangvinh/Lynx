using LynxUI_Main.Models;
using LynxUI_Main.ViewModels;
using System.Diagnostics;

namespace LynxUI_Main.Helpers
{
    public static class ChatMessageHelper
    {
        public static void UpdateLastMessageSummary(ChatListItem chat, List<MessageItem> messages)
        {
            if (chat == null || messages == null || messages.Count == 0)
                return;

            var lastMessage = messages[messages.Count - 1];

            chat.LastMessage = GetMessagePreview(lastMessage);
            chat.LastSenderId = lastMessage.SenderId;
            chat.LastSenderName = lastMessage.SenderName;
            chat.LastMessageType = GetMessageType(lastMessage);
            chat.LastMessageTime = lastMessage.TimeStamp;

            chat.RaiseFormattedLastMessageChanged();
        }

        public static void UpdateChatListItemFromMessages(List<ChatListItem> chatList, Dictionary<int, List<MessageItem>> allMessages)
        {
            foreach (var chat in chatList)
            {
                // Bảo vệ truy cập Dictionary
                if (!allMessages.TryGetValue(chat.Id, out var messages) || messages == null || messages.Count == 0)
                    continue;

                var lastMsg = messages[messages.Count - 1];
                if (lastMsg == null)
                    continue;

                Debug.WriteLine($"[Update] ChatId={chat.Id}, LastMsg={lastMsg.Message}");
                chat.LastMessage = GetMessagePreview(lastMsg);
                chat.LastMessageTime = lastMsg.TimeStamp;
                chat.LastSenderId = lastMsg.SenderId;
                chat.LastSenderName = lastMsg.SenderName;
                chat.LastMessageType = GetMessageType(lastMsg);
                chat.FileName = lastMsg.FileName;

                chat.RaiseFormattedLastMessageChanged();
            }
        }

        private static string GetMessagePreview(MessageItem message)
        {
            if (message == null)
                return "";

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
            if (message == null)
                return "text";

            if (message.IsPicture) return "image";
            if (message.IsVideo) return "video";
            if (message.IsFile) return "file";
            if (message.IsSticker) return "sticker";
            if (message.IsEmoji) return "emoji";
            return "text";
        }
    }
}

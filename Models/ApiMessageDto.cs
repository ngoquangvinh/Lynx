namespace LynxUI_Main.Models
{
    public class ApiMessageDto
    {
        public int MessageId { get; set; }
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SenderName { get; set; }
        public string SenderAvatarUrl { get; set; }
        public string MessageType { get; set; }
        public string MessageText { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileSize { get; set; }
        public string Emoji { get; set; }
        public string Sticker { get; set; }
        public string Status { get; set; }
        public bool IsOwnMessage { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime SentAt { get; set; }
    }
}

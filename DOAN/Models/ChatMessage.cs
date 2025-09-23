namespace DOAN.Models
{
    public class ChatMessage
    {
        public string Text { get; set; } = string.Empty;
        public string? SessionId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}

using DOAN.Respository;
using Microsoft.AspNetCore.Mvc;

namespace DOAN.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { error = "Message is required" });
            }

            var reply = await _chatService.GetBotResponseAsync(request.Message);
            return Ok(new { reply });
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] ChatSendRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return Ok(new { success = false, response = "Nội dung tin nhắn trống." });
            }

            try
            {
                var reply = await _chatService.GetBotResponseAsync(request.Text);
                return Ok(new { success = true, response = reply });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, response = $"Lỗi gọi Gemini: {ex.Message}" });
            }
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    public class ChatSendRequest
    {
        public string Text { get; set; } = string.Empty;
        public string? SessionId { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}

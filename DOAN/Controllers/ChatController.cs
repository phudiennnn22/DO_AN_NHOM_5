using Microsoft.AspNetCore.Mvc;
using DOAN.Models;

namespace DOAN.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;

        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }

        [HttpPost("send")]
        public IActionResult SendMessage([FromBody] ChatMessage message)
        {
            try
            {
                if (string.IsNullOrEmpty(message.Text))
                {
                    return BadRequest(new { success = false, message = "Tin nhắn không được để trống" });
                }

                // Simulate AI response
                var response = GetBotResponse(message.Text);
                
                return Ok(new { 
                    success = true, 
                    response = response,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi xử lý tin nhắn" });
            }
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new { 
                online = true, 
                message = "Hỗ trợ khách hàng đang hoạt động",
                responseTime = "Trong vòng 1 phút"
            });
        }

        private string GetBotResponse(string userMessage)
        {
            var responses = new Dictionary<string, string>
            {
                ["sản phẩm"] = "Tôi có thể giúp bạn tìm hiểu về các sản phẩm. Bạn quan tâm đến sản phẩm nào cụ thể?",
                ["đơn hàng"] = "Tôi có thể hỗ trợ bạn về đơn hàng. Vui lòng cung cấp mã đơn hàng hoặc thông tin liên quan.",
                ["tư vấn"] = "Tôi sẵn sàng tư vấn cho bạn! Bạn đang tìm kiếm sản phẩm gì?",
                ["kỹ thuật"] = "Tôi có thể hỗ trợ bạn về các vấn đề kỹ thuật. Hãy mô tả chi tiết vấn đề bạn gặp phải.",
                ["giá"] = "Bạn có thể xem giá sản phẩm trên trang sản phẩm. Tôi có thể giúp bạn so sánh giá các sản phẩm.",
                ["giao hàng"] = "Chúng tôi giao hàng toàn quốc. Thời gian giao hàng từ 1-3 ngày làm việc.",
                ["bảo hành"] = "Tất cả sản phẩm đều có bảo hành chính hãng. Thời gian bảo hành tùy theo từng sản phẩm.",
                ["đổi trả"] = "Bạn có thể đổi trả sản phẩm trong vòng 7 ngày nếu sản phẩm còn nguyên vẹn.",
                ["iphone"] = "iPhone là sản phẩm cao cấp của Apple với hiệu năng mạnh mẽ và camera chất lượng cao. Bạn quan tâm đến model nào?",
                ["samsung"] = "Samsung Galaxy series có nhiều tùy chọn từ tầm trung đến cao cấp. Bạn muốn tìm hiểu về model nào?",
                ["laptop"] = "Chúng tôi có nhiều dòng laptop từ Apple, Dell, HP với cấu hình đa dạng. Bạn cần laptop cho mục đích gì?",
                ["macbook"] = "MacBook Pro và MacBook Air là lựa chọn tuyệt vời cho công việc và sáng tạo. Bạn cần cấu hình như thế nào?",
                ["airpods"] = "AirPods Pro và AirPods 3 có chất lượng âm thanh tuyệt vời và tính năng chống ồn. Bạn muốn tìm hiểu về model nào?",
                ["đồng hồ"] = "Apple Watch và Samsung Galaxy Watch có nhiều tính năng thông minh. Bạn quan tâm đến tính năng nào?",
                ["xin chào"] = "Xin chào! 👋 Tôi có thể giúp gì cho bạn hôm nay?",
                ["cảm ơn"] = "Không có gì! 😊 Tôi luôn sẵn sàng hỗ trợ bạn. Bạn còn cần giúp gì khác không?",
                ["tạm biệt"] = "Tạm biệt! 👋 Chúc bạn một ngày tốt lành. Hẹn gặp lại bạn lần sau!",
                ["giờ mở cửa"] = "Cửa hàng mở cửa từ 8:00 - 22:00 hàng ngày. Hotline hỗ trợ 24/7: 1900 1234",
                ["địa chỉ"] = "Cửa hàng chúng tôi tại: 123 Đường ABC, Quận 1, TP.HCM. Bạn có thể đến trực tiếp hoặc mua online.",
                ["thanh toán"] = "Chúng tôi hỗ trợ nhiều phương thức thanh toán: COD, chuyển khoản, ví điện tử MoMo, thẻ tín dụng."
            };

            var lowerMessage = userMessage.ToLower();
            
            // Tìm từ khóa phù hợp nhất
            foreach (var kvp in responses)
            {
                if (lowerMessage.Contains(kvp.Key))
                {
                    return kvp.Value;
                }
            }

            // Phản hồi mặc định
            var defaultResponses = new[]
            {
                "Cảm ơn bạn đã liên hệ! Tôi đã ghi nhận câu hỏi của bạn. Đội ngũ hỗ trợ sẽ phản hồi sớm nhất có thể.",
                "Tôi hiểu câu hỏi của bạn. Để được hỗ trợ tốt nhất, bạn có thể gọi hotline 1900 1234 hoặc mô tả chi tiết hơn.",
                "Câu hỏi của bạn rất hay! Tôi sẽ chuyển thông tin này cho đội ngũ chuyên môn để được tư vấn chính xác nhất.",
                "Cảm ơn bạn! Tôi đã ghi nhận yêu cầu. Bạn có thể liên hệ hotline 1900 1234 để được hỗ trợ trực tiếp.",
                "Tôi đã hiểu vấn đề của bạn. Để được giải đáp chi tiết, bạn có thể mô tả thêm hoặc gọi hotline hỗ trợ."
            };

            var random = new Random();
            return defaultResponses[random.Next(defaultResponses.Length)];
        }
    }

    public class ChatMessage
    {
        public string Text { get; set; } = string.Empty;
        public string? SessionId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}

using System.ComponentModel.DataAnnotations;

namespace DOAN.Models
{
    public class ProductReviewViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Đánh giá là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        [Display(Name = "Đánh giá")]
        public int Rating { get; set; }
        
        [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
        [Display(Name = "Tiêu đề đánh giá")]
        public string? Title { get; set; }
        
        [StringLength(1000, ErrorMessage = "Bình luận không được quá 1000 ký tự")]
        [Display(Name = "Bình luận")]
        public string? Comment { get; set; }
        
        public bool IsVerified { get; set; } = false;
    }
    
    public class ProductReviewDisplayViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVerified { get; set; }
        public string RatingStars => new string('★', Rating) + new string('☆', 5 - Rating);
    }
}

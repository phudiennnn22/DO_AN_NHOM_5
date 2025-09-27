using System.ComponentModel.DataAnnotations;

namespace DOAN.Models
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên đăng nhập không được quá 100 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }
        
        [StringLength(15, ErrorMessage = "Số điện thoại không được quá 15 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }
        
        [StringLength(200, ErrorMessage = "Địa chỉ không được quá 200 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        
        // Statistics
        public int TotalOrders { get; set; }
        public int TotalPreOrders { get; set; }
        public int TotalReviews { get; set; }
        public decimal TotalSpent { get; set; }
    }
    
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string CurrentPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; } = string.Empty;
        
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace DOAN.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên đăng nhập không được quá 100 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }

        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
    }
}

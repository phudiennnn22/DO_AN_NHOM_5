using System.ComponentModel.DataAnnotations;

namespace DOAN.Models
{
    public class PreOrderViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, 10, ErrorMessage = "Số lượng phải từ 1 đến 10")]
        public int Quantity { get; set; } = 1;
        
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được quá 15 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Địa chỉ không được quá 200 ký tự")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string? ShippingAddress { get; set; }
        
        [StringLength(500, ErrorMessage = "Ghi chú không được quá 500 ký tự")]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }
        
        [Display(Name = "Số tiền đặt cọc")]
        public decimal DepositAmount { get; set; } = 0;
        
        public DateTime? ExpectedDeliveryDate { get; set; }
        
        // Computed properties
        public decimal TotalAmount => Quantity * Price;
        public decimal RemainingAmount => TotalAmount - DepositAmount;
    }
}

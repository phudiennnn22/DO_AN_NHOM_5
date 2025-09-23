using System.ComponentModel.DataAnnotations;

namespace DOAN.Models
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; } = 0;
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }

    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Stock { get; set; }
        public decimal TotalPrice => Quantity * Price;
        public bool IsAvailable => Stock > 0;
    }

    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(200, ErrorMessage = "Email không được vượt quá 200 ký tự")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        [StringLength(200, ErrorMessage = "Địa chỉ giao hàng không được vượt quá 200 ký tự")]
        public string ShippingAddress { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }

        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; } = 0;
        public decimal TotalAmount { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN.Models
{
    public enum PreOrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        InStock = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }

    public class PreOrder
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PreOrderNumber { get; set; } = string.Empty;
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [Required]
        [StringLength(15)]
        public string CustomerPhone { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? ShippingAddress { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal DepositAmount { get; set; } = 0;
        
        public PreOrderStatus Status { get; set; } = PreOrderStatus.Pending;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? ExpectedDeliveryDate { get; set; }
        
        public DateTime? ActualDeliveryDate { get; set; }
        
        // User ID for logged in users
        public int? UserId { get; set; }
        
        // Session ID for anonymous users
        public string? SessionId { get; set; }
        
        // Navigation properties
        public virtual Product Product { get; set; } = null!;
        public virtual User? User { get; set; }
        
        // Computed properties
        [NotMapped]
        public string StatusText => Status switch
        {
            PreOrderStatus.Pending => "Chờ xác nhận",
            PreOrderStatus.Confirmed => "Đã xác nhận",
            PreOrderStatus.InStock => "Đã có hàng",
            PreOrderStatus.Shipped => "Đã gửi hàng",
            PreOrderStatus.Delivered => "Đã giao hàng",
            PreOrderStatus.Cancelled => "Đã hủy",
            _ => "Không xác định"
        };
        
        [NotMapped]
        public decimal TotalAmount => Quantity * Price;
    }
}

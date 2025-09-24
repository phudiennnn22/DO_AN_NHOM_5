using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN.Models
{
    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }
    
    public enum PaymentStatus
    {
        Pending = 0,
        Paid = 1,
        Failed = 2,
        Refunded = 3
    }
    
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [Required]
        [StringLength(15)]
        public string CustomerPhone { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; } = 0;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? ShippedAt { get; set; }
        
        public DateTime? DeliveredAt { get; set; }
        
        // Session ID for anonymous users
        public string? SessionId { get; set; }
        
        // User ID for logged in users
        public int? UserId { get; set; }
        
        // Navigation properties
        public virtual User? User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        // Computed properties
        [NotMapped]
        public string StatusText => Status switch
        {
            OrderStatus.Pending => "Chờ xác nhận",
            OrderStatus.Confirmed => "Đã xác nhận",
            OrderStatus.Processing => "Đang xử lý",
            OrderStatus.Shipped => "Đã gửi hàng",
            OrderStatus.Delivered => "Đã giao hàng",
            OrderStatus.Cancelled => "Đã hủy",
            _ => "Không xác định"
        };
        
        [NotMapped]
        public string PaymentStatusText => PaymentStatus switch
        {
            PaymentStatus.Pending => "Chờ thanh toán",
            PaymentStatus.Paid => "Đã thanh toán",
            PaymentStatus.Failed => "Thanh toán thất bại",
            PaymentStatus.Refunded => "Đã hoàn tiền",
            _ => "Không xác định"
        };
    }
}

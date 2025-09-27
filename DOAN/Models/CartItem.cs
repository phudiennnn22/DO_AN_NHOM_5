using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        // Session ID for anonymous users
        public string? SessionId { get; set; }
        
        // User ID for logged in users
        public int? UserId { get; set; }
        
        // Navigation properties
        public virtual User? User { get; set; }
        public virtual Product Product { get; set; } = null!;
        
        // Computed properties
        [NotMapped]
        public decimal TotalPrice => Quantity * Price;
    }
}

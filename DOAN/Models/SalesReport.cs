using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN.Models
{
    public class SalesReport
    {
        public int Id { get; set; }
        
        public DateTime ReportDate { get; set; }
        
        public string Period { get; set; } = string.Empty; // Daily, Weekly, Monthly, Yearly
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalRevenue { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalOrders { get; set; }
        
        public int TotalProductsSold { get; set; }
        
        public int TotalCustomers { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal AverageOrderValue { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
    
    public class ProductSalesData
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public decimal Profit { get; set; }
    }
    
    public class CategorySalesData
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int ProductsSold { get; set; }
        public decimal Revenue { get; set; }
        public decimal Percentage { get; set; }
    }
}

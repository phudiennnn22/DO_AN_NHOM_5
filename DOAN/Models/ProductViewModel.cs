namespace DOAN.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public string? Brand { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public bool IsFeatured { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        
        // Computed properties
        public decimal FinalPrice => SalePrice ?? Price;
        public bool IsOnSale => SalePrice.HasValue && SalePrice < Price;
        public decimal DiscountPercentage => IsOnSale ? Math.Round((1 - (SalePrice!.Value / Price)) * 100, 0) : 0;
    }
    
    public class ProductListViewModel
    {
        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12;
        public int TotalProducts { get; set; }
        public int? SelectedCategoryId { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "name";
        public string? SortOrder { get; set; } = "asc";
    }
}

using Microsoft.AspNetCore.Mvc;
using DOAN.Models;
using System.Text.Json;

namespace DOAN.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        
        // Mock data - trong thực tế sẽ sử dụng database
        private static List<Category> _categories = new List<Category>
        {
            new Category { Id = 1, Name = "Điện thoại", Description = "Điện thoại thông minh", IsActive = true },
            new Category { Id = 2, Name = "Laptop", Description = "Máy tính xách tay", IsActive = true },
            new Category { Id = 3, Name = "Phụ kiện", Description = "Phụ kiện điện tử", IsActive = true },
            new Category { Id = 4, Name = "Đồng hồ", Description = "Đồng hồ thông minh", IsActive = true }
        };
        
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "iPhone 15 Pro", Description = "Điện thoại iPhone 15 Pro 128GB với chip A17 Pro mạnh mẽ", Price = 29990000, SalePrice = 27990000, Stock = 50, CategoryId = 1, ImageUrl = "https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=400&h=400&fit=crop&crop=center", Brand = "Apple", IsFeatured = true },
            new Product { Id = 2, Name = "Samsung Galaxy S24 Ultra", Description = "Điện thoại Samsung Galaxy S24 Ultra 256GB với camera 200MP", Price = 32990000, SalePrice = 30990000, Stock = 30, CategoryId = 1, ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400&h=400&fit=crop&crop=center", Brand = "Samsung", IsFeatured = true },
            new Product { Id = 3, Name = "MacBook Pro M3", Description = "Laptop MacBook Pro 14 inch M3 chip với hiệu năng vượt trội", Price = 45990000, SalePrice = 42990000, Stock = 20, CategoryId = 2, ImageUrl = "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=400&h=400&fit=crop&crop=center", Brand = "Apple", IsFeatured = true },
            new Product { Id = 4, Name = "Dell XPS 13", Description = "Laptop Dell XPS 13 Intel i7 với thiết kế cao cấp", Price = 32990000, SalePrice = 29990000, Stock = 25, CategoryId = 2, ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400&h=400&fit=crop&crop=center", Brand = "Dell", IsFeatured = false },
            new Product { Id = 5, Name = "AirPods Pro 2", Description = "Tai nghe AirPods Pro thế hệ 2 với chống ồn chủ động", Price = 5990000, SalePrice = 5490000, Stock = 100, CategoryId = 3, ImageUrl = "https://images.unsplash.com/photo-1606220945770-b5b6c2c55bf1?w=400&h=400&fit=crop&crop=center", Brand = "Apple", IsFeatured = true },
            new Product { Id = 6, Name = "Apple Watch Series 9", Description = "Đồng hồ thông minh Apple Watch Series 9 với chip S9", Price = 8990000, SalePrice = 7990000, Stock = 40, CategoryId = 4, ImageUrl = "https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?w=400&h=400&fit=crop&crop=center", Brand = "Apple", IsFeatured = true },
            new Product { Id = 7, Name = "Samsung Galaxy Watch 6", Description = "Đồng hồ thông minh Samsung Galaxy Watch 6 với Wear OS", Price = 6990000, SalePrice = 5990000, Stock = 35, CategoryId = 4, ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400&h=400&fit=crop&crop=center", Brand = "Samsung", IsFeatured = false },
            new Product { Id = 8, Name = "iPad Air M2", Description = "Máy tính bảng iPad Air 10.9 inch M2 với màn hình Liquid Retina", Price = 15990000, SalePrice = 14990000, Stock = 60, CategoryId = 1, ImageUrl = "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400&h=400&fit=crop&crop=center", Brand = "Apple", IsFeatured = true },
            new Product { Id = 9, Name = "iPhone 14", Description = "Điện thoại iPhone 14 128GB với chip A15 Bionic", Price = 22990000, SalePrice = 19990000, Stock = 45, CategoryId = 1, ImageUrl = "https://images.unsplash.com/photo-1592750475338-74b7b21085ab?w=400&h=400&fit=crop&crop=center", Brand = "Apple", IsFeatured = false },
            new Product { Id = 10, Name = "Samsung Galaxy A54", Description = "Điện thoại Samsung Galaxy A54 128GB với camera 50MP", Price = 8990000, SalePrice = 7990000, Stock = 60, CategoryId = 1, ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400&h=400&fit=crop&crop=center", Brand = "Samsung", IsFeatured = false },
            new Product { Id = 11, Name = "MacBook Air M2", Description = "Laptop MacBook Air 13 inch M2 siêu mỏng nhẹ", Price = 29990000, SalePrice = 27990000, Stock = 35, CategoryId = 2, ImageUrl = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400&h=400&fit=crop&crop=center", Brand = "Apple", IsFeatured = false },
            new Product { Id = 12, Name = "HP Spectre x360", Description = "Laptop HP Spectre x360 2-in-1 với màn hình cảm ứng", Price = 32990000, SalePrice = 29990000, Stock = 20, CategoryId = 2, ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400&h=400&fit=crop&crop=center", Brand = "HP", IsFeatured = false },
            new Product { Id = 13, Name = "Sony WH-1000XM5", Description = "Tai nghe Sony WH-1000XM5 chống ồn hàng đầu thế giới", Price = 8990000, SalePrice = 7990000, Stock = 60, CategoryId = 3, ImageUrl = "https://images.unsplash.com/photo-1583394838336-acd977736f90?w=400&h=400&fit=crop&crop=center", Brand = "Sony", IsFeatured = false },
            new Product { Id = 14, Name = "Bose QuietComfort 45", Description = "Tai nghe Bose QuietComfort 45 chống ồn cao cấp", Price = 7990000, SalePrice = 6990000, Stock = 50, CategoryId = 3, ImageUrl = "https://images.unsplash.com/photo-1583394838336-acd977736f90?w=400&h=400&fit=crop&crop=center", Brand = "Bose", IsFeatured = false },
            new Product { Id = 15, Name = "Garmin Fenix 7", Description = "Đồng hồ thể thao Garmin Fenix 7 với GPS", Price = 12990000, SalePrice = 11990000, Stock = 40, CategoryId = 4, ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400&h=400&fit=crop&crop=center", Brand = "Garmin", IsFeatured = false },
            new Product { Id = 16, Name = "Fitbit Versa 4", Description = "Đồng hồ thông minh Fitbit Versa 4 theo dõi sức khỏe", Price = 5990000, SalePrice = 4990000, Stock = 60, CategoryId = 4, ImageUrl = "https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?w=400&h=400&fit=crop&crop=center", Brand = "Fitbit", IsFeatured = false }
        };

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int? categoryId, string? search, string? sortBy = "name", string? sortOrder = "asc", int page = 1, int pageSize = 12)
        {
            var products = _products.Where(p => p.IsActive).AsQueryable();
            
            // Filter by category
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }
            
            // Search
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                             (p.Description != null && p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                             (p.Brand != null && p.Brand.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }
            
            // Sort
            switch (sortBy.ToLower())
            {
                case "price":
                    products = sortOrder.ToLower() == "desc" ? products.OrderByDescending(p => p.FinalPrice) : products.OrderBy(p => p.FinalPrice);
                    break;
                case "name":
                default:
                    products = sortOrder.ToLower() == "desc" ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name);
                    break;
            }
            
            var totalProducts = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            
            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            var viewModel = new ProductListViewModel
            {
                Products = pagedProducts.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    SalePrice = p.SalePrice,
                    Stock = p.Stock,
                    ImageUrl = p.ImageUrl,
                    Brand = p.Brand,
                    Color = p.Color,
                    Size = p.Size,
                    IsFeatured = p.IsFeatured,
                    CategoryName = _categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name ?? "",
                    CategoryId = p.CategoryId
                }).ToList(),
                Categories = _categories.Where(c => c.IsActive).ToList(),
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalProducts = totalProducts,
                SelectedCategoryId = categoryId,
                SearchTerm = search,
                SortBy = sortBy,
                SortOrder = sortOrder
            };
            
            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id && p.IsActive);
            if (product == null)
            {
                return NotFound();
            }
            
            var relatedProducts = _products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.IsActive)
                .Take(4)
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    SalePrice = p.SalePrice,
                    Stock = p.Stock,
                    ImageUrl = p.ImageUrl,
                    Brand = p.Brand,
                    Color = p.Color,
                    Size = p.Size,
                    IsFeatured = p.IsFeatured,
                    CategoryName = _categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name ?? "",
                    CategoryId = p.CategoryId
                }).ToList();
            
            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SalePrice = product.SalePrice,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                Brand = product.Brand,
                Color = product.Color,
                Size = product.Size,
                IsFeatured = product.IsFeatured,
                CategoryName = _categories.FirstOrDefault(c => c.Id == product.CategoryId)?.Name ?? "",
                CategoryId = product.CategoryId
            };
            
            ViewBag.RelatedProducts = relatedProducts;
            
            return View(viewModel);
        }
    }
}

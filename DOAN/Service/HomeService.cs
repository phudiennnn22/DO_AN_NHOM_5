using DOAN.Data;
using DOAN.Models;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Service
{
    public class HomeService : IHomeService
    {
        private readonly AppDbContext _context;

        public HomeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductViewModel>> GetFeaturedProductsAsync(int take = 8)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.IsFeatured && p.IsActive)
                .Take(take)
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
                    CategoryName = p.Category!.Name,
                    CategoryId = p.CategoryId
                })
                .ToListAsync();
        }

        public async Task<List<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .ToListAsync();
        }
    }
}

using DOAN.Data;
using DOAN.Models;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Service
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductListViewModel> GetProductsAsync(int? categoryId, string? search, string? sortBy, string? sortOrder, int page, int pageSize)
        {
            var productsQuery = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(search) ||
                                                         (p.Description != null && p.Description.Contains(search)) ||
                                                         (p.Brand != null && p.Brand.Contains(search)));
            }

            switch ((sortBy ?? "name").ToLower())
            {
                case "price":
                    productsQuery = (sortOrder ?? "asc").ToLower() == "desc" ? productsQuery.OrderByDescending(p => p.FinalPrice) : productsQuery.OrderBy(p => p.FinalPrice);
                    break;
                case "name":
                default:
                    productsQuery = (sortOrder ?? "asc").ToLower() == "desc" ? productsQuery.OrderByDescending(p => p.Name) : productsQuery.OrderBy(p => p.Name);
                    break;
            }

            var totalProducts = await productsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var pagedProducts = await productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await _context.Categories.AsNoTracking().Where(c => c.IsActive).ToListAsync();

            return new ProductListViewModel
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
                    CategoryName = p.Category?.Name ?? string.Empty,
                    CategoryId = p.CategoryId
                }).ToList(),
                Categories = categories,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalProducts = totalProducts,
                SelectedCategoryId = categoryId,
                SearchTerm = search,
                SortBy = sortBy,
                SortOrder = sortOrder
            };
        }

        public async Task<ProductViewModel?> GetProductDetailsAsync(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null) return null;

            return new ProductViewModel
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
                CategoryName = product.Category?.Name ?? string.Empty,
                CategoryId = product.CategoryId
            };
        }

        public async Task<List<ProductViewModel>> GetRelatedProductsAsync(int categoryId, int excludeProductId)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId && p.Id != excludeProductId && p.IsActive)
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
                    CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                    CategoryId = p.CategoryId
                }).ToListAsync();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                product.UpdatedAt = DateTime.Now;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateProductAsync(Product product)
        {
            try
            {
                product.CreatedAt = DateTime.Now;
                product.UpdatedAt = DateTime.Now;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null) return false;

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

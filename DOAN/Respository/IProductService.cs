using DOAN.Models;

namespace DOAN.Service
{
    public interface IProductService
    {
        Task<ProductListViewModel> GetProductsAsync(int? categoryId, string? search, string? sortBy, string? sortOrder, int page, int pageSize);
        Task<ProductViewModel?> GetProductDetailsAsync(int id);
        Task<List<ProductViewModel>> GetRelatedProductsAsync(int categoryId, int excludeProductId);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<bool> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
    }
}

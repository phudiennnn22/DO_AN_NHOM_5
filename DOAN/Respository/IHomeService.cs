using DOAN.Models;

namespace DOAN.Service
{
    public interface IHomeService
    {
        Task<List<ProductViewModel>> GetFeaturedProductsAsync(int take = 8);
        Task<List<Category>> GetActiveCategoriesAsync();
    }
}

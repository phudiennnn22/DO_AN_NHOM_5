using DOAN.Models;

namespace DOAN.Respository
{
    public interface IProductReviewService
    {
        Task<ProductReview?> CreateReviewAsync(ProductReviewViewModel model, int userId);
        Task<ProductReview?> GetReviewByIdAsync(int id);
        Task<IEnumerable<ProductReview>> GetReviewsByProductIdAsync(int productId);
        Task<IEnumerable<ProductReview>> GetReviewsByUserIdAsync(int userId);
        Task<IEnumerable<ProductReview>> GetAllReviewsAsync();
        Task<bool> UpdateReviewAsync(int id, ProductReviewViewModel model);
        Task<bool> DeleteReviewAsync(int id);
        Task<bool> ApproveReviewAsync(int id);
        Task<bool> RejectReviewAsync(int id);
        Task<bool> HasUserReviewedProductAsync(int userId, int productId);
        Task<decimal> GetAverageRatingAsync(int productId);
        Task<int> GetReviewCountAsync(int productId);
    }
}

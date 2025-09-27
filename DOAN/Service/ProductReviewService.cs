using DOAN.Data;
using DOAN.Models;
using DOAN.Respository;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Service
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly AppDbContext _context;

        public ProductReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductReview?> CreateReviewAsync(ProductReviewViewModel model, int userId)
        {
            // Check if user already reviewed this product
            if (await HasUserReviewedProductAsync(userId, model.ProductId))
            {
                return null;
            }

            var review = new ProductReview
            {
                ProductId = model.ProductId,
                UserId = userId,
                Rating = model.Rating,
                Title = model.Title,
                Comment = model.Comment,
                IsVerified = model.IsVerified,
                IsApproved = true,
                CreatedAt = DateTime.Now
            };

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<ProductReview?> GetReviewByIdAsync(int id)
        {
            return await _context.ProductReviews
                .Include(pr => pr.Product)
                .Include(pr => pr.User)
                .FirstOrDefaultAsync(pr => pr.Id == id);
        }

        public async Task<IEnumerable<ProductReview>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.ProductReviews
                .Include(pr => pr.User)
                .Where(pr => pr.ProductId == productId && pr.IsApproved)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductReview>> GetReviewsByUserIdAsync(int userId)
        {
            return await _context.ProductReviews
                .Include(pr => pr.Product)
                .Where(pr => pr.UserId == userId)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductReview>> GetAllReviewsAsync()
        {
            return await _context.ProductReviews
                .Include(pr => pr.Product)
                .Include(pr => pr.User)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateReviewAsync(int id, ProductReviewViewModel model)
        {
            var review = await _context.ProductReviews.FindAsync(id);
            if (review == null) return false;

            review.Rating = model.Rating;
            review.Title = model.Title;
            review.Comment = model.Comment;
            review.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _context.ProductReviews.FindAsync(id);
            if (review == null) return false;

            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveReviewAsync(int id)
        {
            var review = await _context.ProductReviews.FindAsync(id);
            if (review == null) return false;

            review.IsApproved = true;
            review.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectReviewAsync(int id)
        {
            var review = await _context.ProductReviews.FindAsync(id);
            if (review == null) return false;

            review.IsApproved = false;
            review.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasUserReviewedProductAsync(int userId, int productId)
        {
            return await _context.ProductReviews
                .AnyAsync(pr => pr.UserId == userId && pr.ProductId == productId);
        }

        public async Task<decimal> GetAverageRatingAsync(int productId)
        {
            var reviews = await _context.ProductReviews
                .Where(pr => pr.ProductId == productId && pr.IsApproved)
                .ToListAsync();

            if (!reviews.Any()) return 0;

            return Math.Round((decimal)reviews.Average(r => r.Rating), 1);
        }

        public async Task<int> GetReviewCountAsync(int productId)
        {
            return await _context.ProductReviews
                .CountAsync(pr => pr.ProductId == productId && pr.IsApproved);
        }
    }
}

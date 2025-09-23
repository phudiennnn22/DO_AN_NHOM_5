using Microsoft.AspNetCore.Mvc;
using DOAN.Models;
using DOAN.Service;
using DOAN.Respository;

namespace DOAN.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        private readonly IProductReviewService _reviewService;

        public ProductController(ILogger<ProductController> logger, IProductService productService, IProductReviewService reviewService)
        {
            _logger = logger;
            _productService = productService;
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index(int? categoryId, string? search, string? sortBy = "name", string? sortOrder = "asc", int page = 1, int pageSize = 12)
        {
            var viewModel = await _productService.GetProductsAsync(categoryId, search, sortBy, sortOrder, page, pageSize);
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductDetailsAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var relatedProducts = await _productService.GetRelatedProductsAsync(product.CategoryId, id);
            ViewBag.RelatedProducts = relatedProducts;

            // Get product reviews
            var reviews = await _reviewService.GetReviewsByProductIdAsync(id);
            var averageRating = await _reviewService.GetAverageRatingAsync(id);
            var reviewCount = await _reviewService.GetReviewCountAsync(id);

            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = averageRating;
            ViewBag.ReviewCount = reviewCount;

            // Check if user can review
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                ViewBag.CanReview = !await _reviewService.HasUserReviewedProductAsync(userId.Value, id);
            }
            else
            {
                ViewBag.CanReview = false;
            }

            return View(product);
        }
    }
}

using DOAN.Models;
using DOAN.Respository;
using DOAN.Service;
using Microsoft.AspNetCore.Mvc;

namespace DOAN.Controllers
{
    public class ProductReviewController : Controller
    {
        private readonly IProductReviewService _reviewService;
        private readonly IProductService _productService;

        public ProductReviewController(IProductReviewService reviewService, IProductService productService)
        {
            _reviewService = reviewService;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Check if user already reviewed this product
            var hasReviewed = await _reviewService.HasUserReviewedProductAsync(userId.Value, productId);
            if (hasReviewed)
            {
                TempData["ErrorMessage"] = "Bạn đã đánh giá sản phẩm này rồi.";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            var model = new ProductReviewViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductImage = product.ImageUrl ?? ""
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductReviewViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                var product = await _productService.GetProductByIdAsync(model.ProductId);
                if (product != null)
                {
                    model.ProductName = product.Name;
                    model.ProductImage = product.ImageUrl ?? "";
                }
                return View(model);
            }

            var review = await _reviewService.CreateReviewAsync(model, userId.Value);
            if (review == null)
            {
                ModelState.AddModelError(string.Empty, "Bạn đã đánh giá sản phẩm này rồi hoặc có lỗi xảy ra.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Cảm ơn bạn đã đánh giá sản phẩm!";
            return RedirectToAction("Details", "Product", new { id = model.ProductId });
        }

        [HttpGet]
        public async Task<IActionResult> MyReviews()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var reviews = await _reviewService.GetReviewsByUserIdAsync(userId.Value);
            return View(reviews);
        }

        [HttpGet]
        public async Task<IActionResult> ProductReviews(int productId)
        {
            var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
            var product = await _productService.GetProductByIdAsync(productId);
            
            ViewBag.Product = product;
            ViewBag.AverageRating = await _reviewService.GetAverageRatingAsync(productId);
            ViewBag.ReviewCount = await _reviewService.GetReviewCountAsync(productId);
            
            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null || review.UserId != userId.Value)
            {
                return NotFound();
            }

            var success = await _reviewService.DeleteReviewAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã xóa đánh giá thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa đánh giá.";
            }

            return RedirectToAction("MyReviews");
        }

        // Admin actions
        [HttpGet]
        public async Task<IActionResult> AdminIndex()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var success = await _reviewService.ApproveReviewAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã duyệt đánh giá thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể duyệt đánh giá.";
            }

            return RedirectToAction("AdminIndex");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var success = await _reviewService.RejectReviewAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã từ chối đánh giá thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể từ chối đánh giá.";
            }

            return RedirectToAction("AdminIndex");
        }
    }
}

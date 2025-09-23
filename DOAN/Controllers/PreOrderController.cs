using DOAN.Models;
using DOAN.Respository;
using DOAN.Service;
using Microsoft.AspNetCore.Mvc;

namespace DOAN.Controllers
{
    public class PreOrderController : Controller
    {
        private readonly IPreOrderService _preOrderService;
        private readonly IProductService _productService;

        public PreOrderController(IPreOrderService preOrderService, IProductService productService)
        {
            _preOrderService = preOrderService;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var model = new PreOrderViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductImage = product.ImageUrl ?? "",
                Price = product.FinalPrice
            };

            // Pre-fill user info if logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                // You can get user info from AuthService if needed
                model.CustomerName = HttpContext.Session.GetString("FullName") ?? "";
                model.CustomerEmail = HttpContext.Session.GetString("Email") ?? "";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PreOrderViewModel model)
        {
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

            var userId = HttpContext.Session.GetInt32("UserId");
            var sessionId = HttpContext.Session.Id;

            var preOrder = await _preOrderService.CreatePreOrderAsync(model, userId, sessionId);
            if (preOrder == null)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi tạo đơn đặt hàng trước. Vui lòng thử lại.");
                return View(model);
            }

            TempData["SuccessMessage"] = $"Đặt hàng trước thành công! Mã đơn hàng: {preOrder.PreOrderNumber}";
            return RedirectToAction("Details", new { id = preOrder.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var preOrder = await _preOrderService.GetPreOrderByIdAsync(id);
            if (preOrder == null)
            {
                return NotFound();
            }

            // Check if user can view this pre-order
            var userId = HttpContext.Session.GetInt32("UserId");
            if (preOrder.UserId != userId && preOrder.SessionId != HttpContext.Session.Id)
            {
                return Forbid();
            }

            return View(preOrder);
        }

        [HttpGet]
        public async Task<IActionResult> MyPreOrders()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var preOrders = await _preOrderService.GetPreOrdersByUserIdAsync(userId.Value);
            return View(preOrders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _preOrderService.CancelPreOrderAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã hủy đơn đặt hàng trước thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn đặt hàng trước.";
            }

            return RedirectToAction("MyPreOrders");
        }

        // Admin actions
        [HttpGet]
        public async Task<IActionResult> AdminIndex()
        {
            var preOrders = await _preOrderService.GetAllPreOrdersAsync();
            return View(preOrders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, PreOrderStatus status)
        {
            var success = await _preOrderService.UpdatePreOrderStatusAsync(id, status);
            if (success)
            {
                TempData["SuccessMessage"] = "Cập nhật trạng thái thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái.";
            }

            return RedirectToAction("AdminIndex");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using DOAN.Service;
using DOAN.Models;

namespace DOAN.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var sessionId = GetSessionId();
            var viewModel = await _cartService.GetCartAsync(sessionId);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var sessionId = GetSessionId();
            var result = await _cartService.AddToCartAsync(sessionId, productId, quantity);
            return Json(new { result.success, result.message, cartCount = result.cartCount });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var sessionId = GetSessionId();
            var result = await _cartService.UpdateQuantityAsync(sessionId, cartItemId, quantity);
            return Json(new { result.success, result.message, data = result.data });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var sessionId = GetSessionId();
            var result = await _cartService.RemoveFromCartAsync(sessionId, cartItemId);
            return Json(new { result.success, result.message, data = result.data });
        }

        public async Task<IActionResult> Checkout()
        {
            var sessionId = GetSessionId();
            var viewModel = await _cartService.GetCheckoutAsync(sessionId);
            if (viewModel == null) return RedirectToAction("Index");
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Checkout", model);

            var sessionId = GetSessionId();
            var result = await _cartService.PlaceOrderAsync(sessionId, model);

            if (!result.success) return RedirectToAction("Index");

            TempData["SuccessMessage"] = $"{result.message}! Mã đơn hàng: {result.orderNumber}";
            return RedirectToAction("OrderSuccess", new { orderNumber = result.orderNumber });
        }

        public IActionResult OrderSuccess(string orderNumber)
        {
            ViewBag.OrderNumber = orderNumber;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var sessionId = GetSessionId();
            var count = await _cartService.GetCartCountAsync(sessionId);
            return Json(new { count });
        }

        private string GetSessionId()
        {
            var sessionId = HttpContext.Session.GetString("SessionId");
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("SessionId", sessionId);
            }
            return sessionId;
        }
    }
}

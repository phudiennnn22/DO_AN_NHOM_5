using Microsoft.AspNetCore.Mvc;
using DOAN.Models;
using System.Text.Json;

namespace DOAN.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        
        // Mock data - trong thực tế sẽ sử dụng database
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "iPhone 15 Pro", Description = "Điện thoại iPhone 15 Pro 128GB", Price = 29990000, SalePrice = 27990000, Stock = 50, CategoryId = 1, ImageUrl = "https://via.placeholder.com/300x300?text=iPhone+15+Pro", Brand = "Apple", IsFeatured = true },
            new Product { Id = 2, Name = "Samsung Galaxy S24", Description = "Điện thoại Samsung Galaxy S24 256GB", Price = 22990000, Stock = 30, CategoryId = 1, ImageUrl = "https://via.placeholder.com/300x300?text=Galaxy+S24", Brand = "Samsung", IsFeatured = true },
            new Product { Id = 3, Name = "MacBook Pro M3", Description = "Laptop MacBook Pro 14 inch M3", Price = 45990000, SalePrice = 42990000, Stock = 20, CategoryId = 2, ImageUrl = "https://via.placeholder.com/300x300?text=MacBook+Pro", Brand = "Apple", IsFeatured = true },
            new Product { Id = 4, Name = "Dell XPS 13", Description = "Laptop Dell XPS 13 2024", Price = 32990000, Stock = 25, CategoryId = 2, ImageUrl = "https://via.placeholder.com/300x300?text=Dell+XPS", Brand = "Dell", IsFeatured = false },
            new Product { Id = 5, Name = "AirPods Pro", Description = "Tai nghe AirPods Pro 2", Price = 5990000, Stock = 100, CategoryId = 3, ImageUrl = "https://via.placeholder.com/300x300?text=AirPods+Pro", Brand = "Apple", IsFeatured = true },
            new Product { Id = 6, Name = "Apple Watch Series 9", Description = "Đồng hồ Apple Watch Series 9", Price = 8990000, SalePrice = 7990000, Stock = 40, CategoryId = 4, ImageUrl = "https://via.placeholder.com/300x300?text=Apple+Watch", Brand = "Apple", IsFeatured = true },
            new Product { Id = 7, Name = "Samsung Galaxy Watch 6", Description = "Đồng hồ Samsung Galaxy Watch 6", Price = 6990000, Stock = 35, CategoryId = 4, ImageUrl = "https://via.placeholder.com/300x300?text=Galaxy+Watch", Brand = "Samsung", IsFeatured = false },
            new Product { Id = 8, Name = "iPad Air", Description = "Máy tính bảng iPad Air 2024", Price = 15990000, SalePrice = 14990000, Stock = 60, CategoryId = 1, ImageUrl = "https://via.placeholder.com/300x300?text=iPad+Air", Brand = "Apple", IsFeatured = true }
        };
        
        private static List<CartItem> _cartItems = new List<CartItem>();

        public CartController(ILogger<CartController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var sessionId = GetSessionId();
            var cartItems = _cartItems.Where(c => c.SessionId == sessionId).ToList();
            
            var viewModel = new CartViewModel
            {
                Items = cartItems.Select(c => new CartItemViewModel
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = _products.FirstOrDefault(p => p.Id == c.ProductId)?.Name ?? "",
                    ProductImageUrl = _products.FirstOrDefault(p => p.Id == c.ProductId)?.ImageUrl,
                    Price = c.Price,
                    Quantity = c.Quantity,
                    Stock = _products.FirstOrDefault(p => p.Id == c.ProductId)?.Stock ?? 0
                }).ToList(),
                SubTotal = cartItems.Sum(c => c.TotalPrice),
                ShippingFee = cartItems.Any() ? 50000 : 0, // Phí ship 50k nếu có sản phẩm
                TotalAmount = cartItems.Sum(c => c.TotalPrice) + (cartItems.Any() ? 50000 : 0),
                TotalItems = cartItems.Sum(c => c.Quantity)
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _products.FirstOrDefault(p => p.Id == productId && p.IsActive);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }
            
            if (product.Stock < quantity)
            {
                return Json(new { success = false, message = "Số lượng sản phẩm không đủ" });
            }
            
            var sessionId = GetSessionId();
            var existingItem = _cartItems.FirstOrDefault(c => c.SessionId == sessionId && c.ProductId == productId);
            
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.UpdatedAt = DateTime.Now;
            }
            else
            {
                var cartItem = new CartItem
                {
                    Id = _cartItems.Count + 1,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.FinalPrice,
                    SessionId = sessionId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _cartItems.Add(cartItem);
            }
            
            var cartCount = _cartItems.Where(c => c.SessionId == sessionId).Sum(c => c.Quantity);
            
            return Json(new { success = true, message = "Đã thêm vào giỏ hàng", cartCount = cartCount });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemId, int quantity)
        {
            var sessionId = GetSessionId();
            var cartItem = _cartItems.FirstOrDefault(c => c.Id == cartItemId && c.SessionId == sessionId);
            
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng" });
            }
            
            var product = _products.FirstOrDefault(p => p.Id == cartItem.ProductId);
            if (product == null || product.Stock < quantity)
            {
                return Json(new { success = false, message = "Số lượng sản phẩm không đủ" });
            }
            
            cartItem.Quantity = quantity;
            cartItem.UpdatedAt = DateTime.Now;
            
            var cartCount = _cartItems.Where(c => c.SessionId == sessionId).Sum(c => c.Quantity);
            var totalPrice = cartItem.TotalPrice;
            var subTotal = _cartItems.Where(c => c.SessionId == sessionId).Sum(c => c.TotalPrice);
            var shippingFee = subTotal > 0 ? 50000 : 0;
            var totalAmount = subTotal + shippingFee;
            
            return Json(new { 
                success = true, 
                cartCount = cartCount,
                totalPrice = totalPrice,
                subTotal = subTotal,
                shippingFee = shippingFee,
                totalAmount = totalAmount
            });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            var sessionId = GetSessionId();
            var cartItem = _cartItems.FirstOrDefault(c => c.Id == cartItemId && c.SessionId == sessionId);
            
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng" });
            }
            
            _cartItems.Remove(cartItem);
            
            var cartCount = _cartItems.Where(c => c.SessionId == sessionId).Sum(c => c.Quantity);
            var subTotal = _cartItems.Where(c => c.SessionId == sessionId).Sum(c => c.TotalPrice);
            var shippingFee = subTotal > 0 ? 50000 : 0;
            var totalAmount = subTotal + shippingFee;
            
            return Json(new { 
                success = true, 
                message = "Đã xóa sản phẩm khỏi giỏ hàng",
                cartCount = cartCount,
                subTotal = subTotal,
                shippingFee = shippingFee,
                totalAmount = totalAmount
            });
        }

        public IActionResult Checkout()
        {
            var sessionId = GetSessionId();
            var cartItems = _cartItems.Where(c => c.SessionId == sessionId).ToList();
            
            if (!cartItems.Any())
            {
                return RedirectToAction("Index");
            }
            
            var viewModel = new CheckoutViewModel
            {
                CartItems = cartItems.Select(c => new CartItemViewModel
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = _products.FirstOrDefault(p => p.Id == c.ProductId)?.Name ?? "",
                    ProductImageUrl = _products.FirstOrDefault(p => p.Id == c.ProductId)?.ImageUrl,
                    Price = c.Price,
                    Quantity = c.Quantity,
                    Stock = _products.FirstOrDefault(p => p.Id == c.ProductId)?.Stock ?? 0
                }).ToList(),
                SubTotal = cartItems.Sum(c => c.TotalPrice),
                ShippingFee = 50000,
                TotalAmount = cartItems.Sum(c => c.TotalPrice) + 50000
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Checkout", model);
            }
            
            var sessionId = GetSessionId();
            var cartItems = _cartItems.Where(c => c.SessionId == sessionId).ToList();
            
            if (!cartItems.Any())
            {
                return RedirectToAction("Index");
            }
            
            // Tạo đơn hàng
            var order = new Order
            {
                Id = 1, // Trong thực tế sẽ tự động tăng
                OrderNumber = "ORD" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                ShippingAddress = model.ShippingAddress,
                Notes = model.Notes,
                SubTotal = model.SubTotal,
                ShippingFee = model.ShippingFee,
                TotalAmount = model.TotalAmount,
                Status = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                SessionId = sessionId,
                CreatedAt = DateTime.Now
            };
            
            // Tạo chi tiết đơn hàng
            var orderItems = cartItems.Select(c => new OrderItem
            {
                Id = 1, // Trong thực tế sẽ tự động tăng
                OrderId = order.Id,
                ProductId = c.ProductId,
                ProductName = _products.FirstOrDefault(p => p.Id == c.ProductId)?.Name ?? "",
                Quantity = c.Quantity,
                Price = c.Price,
                ProductImageUrl = _products.FirstOrDefault(p => p.Id == c.ProductId)?.ImageUrl
            }).ToList();
            
            // Xóa giỏ hàng
            _cartItems.RemoveAll(c => c.SessionId == sessionId);
            
            // Trong thực tế sẽ lưu vào database
            // _context.Orders.Add(order);
            // _context.OrderItems.AddRange(orderItems);
            // _context.SaveChanges();
            
            TempData["SuccessMessage"] = "Đặt hàng thành công! Mã đơn hàng: " + order.OrderNumber;
            
            return RedirectToAction("OrderSuccess", new { orderNumber = order.OrderNumber });
        }

        public IActionResult OrderSuccess(string orderNumber)
        {
            ViewBag.OrderNumber = orderNumber;
            return View();
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var sessionId = GetSessionId();
            var cartCount = _cartItems.Where(c => c.SessionId == sessionId).Sum(c => c.Quantity);
            return Json(new { count = cartCount });
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

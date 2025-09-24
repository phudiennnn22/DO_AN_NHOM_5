using DOAN.Data;
using DOAN.Models;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Service
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CartService> _logger;

        public CartService(AppDbContext context, ILogger<CartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CartViewModel> GetCartAsync(string sessionId)
        {
            var cartItems = await _context.CartItems
                .AsNoTracking()
                .Include(c => c.Product)
                .Where(c => c.SessionId == sessionId)
                .ToListAsync();

            return new CartViewModel
            {
                Items = cartItems.Select(c => new CartItemViewModel
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product?.Name ?? string.Empty,
                    ProductImageUrl = c.Product?.ImageUrl,
                    Price = c.Price,
                    Quantity = c.Quantity,
                    Stock = c.Product?.Stock ?? 0
                }).ToList(),
                SubTotal = cartItems.Sum(c => c.TotalPrice),
                ShippingFee = cartItems.Any() ? 50000 : 0,
                TotalAmount = cartItems.Sum(c => c.TotalPrice) + (cartItems.Any() ? 50000 : 0),
                TotalItems = cartItems.Sum(c => c.Quantity)
            };
        }

        public async Task<(bool success, string message, int cartCount)> AddToCartAsync(string sessionId, int productId, int quantity)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);
            if (product == null)
                return (false, "Sản phẩm không tồn tại", 0);

            if (product.Stock < quantity)
                return (false, "Số lượng sản phẩm không đủ", 0);

            var existingItem = await _context.CartItems.FirstOrDefaultAsync(c => c.SessionId == sessionId && c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.UpdatedAt = DateTime.Now;
                _context.CartItems.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.FinalPrice,
                    SessionId = sessionId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await _context.CartItems.AddAsync(cartItem);
            }

            await _context.SaveChangesAsync();

            var cartCount = await _context.CartItems.Where(c => c.SessionId == sessionId).SumAsync(c => c.Quantity);
            return (true, "Đã thêm vào giỏ hàng", cartCount);
        }

        public async Task<(bool success, string message, object data)> UpdateQuantityAsync(string sessionId, int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.SessionId == sessionId);
            if (cartItem == null)
                return (false, "Sản phẩm không tồn tại trong giỏ hàng", new { });

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);
            if (product == null || product.Stock < quantity)
                return (false, "Số lượng sản phẩm không đủ", new { });

            cartItem.Quantity = quantity;
            cartItem.UpdatedAt = DateTime.Now;
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();

            var cartCount = await _context.CartItems.Where(c => c.SessionId == sessionId).SumAsync(c => c.Quantity);
            var subTotal = await _context.CartItems.Where(c => c.SessionId == sessionId).SumAsync(c => c.TotalPrice);
            var shippingFee = subTotal > 0 ? 50000 : 0;
            var totalAmount = subTotal + shippingFee;

            return (true, "Cập nhật thành công", new
            {
                cartCount,
                totalPrice = cartItem.TotalPrice,
                subTotal,
                shippingFee,
                totalAmount
            });
        }

        public async Task<(bool success, string message, object data)> RemoveFromCartAsync(string sessionId, int cartItemId)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.SessionId == sessionId);
            if (cartItem == null)
                return (false, "Sản phẩm không tồn tại trong giỏ hàng", new { });

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            var cartCount = await _context.CartItems.Where(c => c.SessionId == sessionId).SumAsync(c => c.Quantity);
            var subTotal = await _context.CartItems.Where(c => c.SessionId == sessionId).SumAsync(c => c.TotalPrice);
            var shippingFee = subTotal > 0 ? 50000 : 0;
            var totalAmount = subTotal + shippingFee;

            return (true, "Đã xóa sản phẩm khỏi giỏ hàng", new
            {
                cartCount,
                subTotal,
                shippingFee,
                totalAmount
            });
        }

        public async Task<CheckoutViewModel?> GetCheckoutAsync(string sessionId)
        {
            var cartItems = await _context.CartItems
                .AsNoTracking()
                .Include(c => c.Product)
                .Where(c => c.SessionId == sessionId)
                .ToListAsync();

            if (!cartItems.Any())
                return null;

            return new CheckoutViewModel
            {
                CartItems = cartItems.Select(c => new CartItemViewModel
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product?.Name ?? string.Empty,
                    ProductImageUrl = c.Product?.ImageUrl,
                    Price = c.Price,
                    Quantity = c.Quantity,
                    Stock = c.Product?.Stock ?? 0
                }).ToList(),
                SubTotal = cartItems.Sum(c => c.TotalPrice),
                ShippingFee = 50000,
                TotalAmount = cartItems.Sum(c => c.TotalPrice) + 50000
            };
        }

        public async Task<(bool success, string orderNumber, string message)> PlaceOrderAsync(string sessionId, CheckoutViewModel model)
        {
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.SessionId == sessionId)
                .ToListAsync();

            if (!cartItems.Any())
                return (false, string.Empty, "Giỏ hàng trống");

            var order = new Order
            {
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

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var orderItems = cartItems.Select(c => new OrderItem
            {
                OrderId = order.Id,
                ProductId = c.ProductId,
                ProductName = c.Product?.Name ?? string.Empty,
                Quantity = c.Quantity,
                Price = c.Price,
                ProductImageUrl = c.Product?.ImageUrl
            }).ToList();

            await _context.OrderItems.AddRangeAsync(orderItems);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return (true, order.OrderNumber, "Đặt hàng thành công");
        }

        public async Task<int> GetCartCountAsync(string sessionId)
        {
            return await _context.CartItems.Where(c => c.SessionId == sessionId).SumAsync(c => c.Quantity);
        }
    }
}

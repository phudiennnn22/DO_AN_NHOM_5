using DOAN.Models;

namespace DOAN.Service
{
    public interface ICartService
    {
        Task<CartViewModel> GetCartAsync(string sessionId);
        Task<(bool success, string message, int cartCount)> AddToCartAsync(string sessionId, int productId, int quantity);
        Task<(bool success, string message, object data)> UpdateQuantityAsync(string sessionId, int cartItemId, int quantity);
        Task<(bool success, string message, object data)> RemoveFromCartAsync(string sessionId, int cartItemId);
        Task<CheckoutViewModel?> GetCheckoutAsync(string sessionId);
        Task<(bool success, string orderNumber, string message)> PlaceOrderAsync(string sessionId, CheckoutViewModel model);
        Task<int> GetCartCountAsync(string sessionId);
    }
}

using DOAN.Models;

namespace DOAN.Respository
{
    public interface IPreOrderService
    {
        Task<PreOrder?> CreatePreOrderAsync(PreOrderViewModel model, int? userId = null, string? sessionId = null);
        Task<PreOrder?> GetPreOrderByIdAsync(int id);
        Task<IEnumerable<PreOrder>> GetPreOrdersByUserIdAsync(int userId);
        Task<IEnumerable<PreOrder>> GetPreOrdersBySessionIdAsync(string sessionId);
        Task<IEnumerable<PreOrder>> GetAllPreOrdersAsync();
        Task<bool> UpdatePreOrderStatusAsync(int id, PreOrderStatus status);
        Task<bool> CancelPreOrderAsync(int id);
        Task<bool> DeletePreOrderAsync(int id);
        Task<string> GeneratePreOrderNumberAsync();
    }
}

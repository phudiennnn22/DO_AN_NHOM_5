using DOAN.Data;
using DOAN.Models;
using DOAN.Respository;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Service
{
    public class PreOrderService : IPreOrderService
    {
        private readonly AppDbContext _context;

        public PreOrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PreOrder?> CreatePreOrderAsync(PreOrderViewModel model, int? userId = null, string? sessionId = null)
        {
            var product = await _context.Products.FindAsync(model.ProductId);
            if (product == null) return null;

            var preOrder = new PreOrder
            {
                PreOrderNumber = await GeneratePreOrderNumberAsync(),
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                Price = model.Price,
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                ShippingAddress = model.ShippingAddress,
                Notes = model.Notes,
                DepositAmount = model.DepositAmount,
                ExpectedDeliveryDate = model.ExpectedDeliveryDate,
                UserId = userId,
                SessionId = sessionId,
                Status = PreOrderStatus.Pending,
                CreatedAt = DateTime.Now
            };

            _context.PreOrders.Add(preOrder);
            await _context.SaveChangesAsync();
            return preOrder;
        }

        public async Task<PreOrder?> GetPreOrderByIdAsync(int id)
        {
            return await _context.PreOrders
                .Include(po => po.Product)
                .Include(po => po.User)
                .FirstOrDefaultAsync(po => po.Id == id);
        }

        public async Task<IEnumerable<PreOrder>> GetPreOrdersByUserIdAsync(int userId)
        {
            return await _context.PreOrders
                .Include(po => po.Product)
                .Where(po => po.UserId == userId)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PreOrder>> GetPreOrdersBySessionIdAsync(string sessionId)
        {
            return await _context.PreOrders
                .Include(po => po.Product)
                .Where(po => po.SessionId == sessionId)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PreOrder>> GetAllPreOrdersAsync()
        {
            return await _context.PreOrders
                .Include(po => po.Product)
                .Include(po => po.User)
                .OrderByDescending(po => po.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdatePreOrderStatusAsync(int id, PreOrderStatus status)
        {
            var preOrder = await _context.PreOrders.FindAsync(id);
            if (preOrder == null) return false;

            preOrder.Status = status;
            preOrder.UpdatedAt = DateTime.Now;

            if (status == PreOrderStatus.Delivered)
            {
                preOrder.ActualDeliveryDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelPreOrderAsync(int id)
        {
            var preOrder = await _context.PreOrders.FindAsync(id);
            if (preOrder == null) return false;

            preOrder.Status = PreOrderStatus.Cancelled;
            preOrder.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePreOrderAsync(int id)
        {
            var preOrder = await _context.PreOrders.FindAsync(id);
            if (preOrder == null) return false;

            _context.PreOrders.Remove(preOrder);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GeneratePreOrderNumberAsync()
        {
            var today = DateTime.Now.ToString("yyyyMMdd");
            var count = await _context.PreOrders
                .Where(po => po.PreOrderNumber.StartsWith($"PO{today}"))
                .CountAsync();
            
            return $"PO{today}{(count + 1):D4}";
        }
    }
}

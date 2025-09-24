using DOAN.Data;
using DOAN.Models;
using DOAN.Respository;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Service
{
    public class SalesReportService : ISalesReportService
    {
        private readonly AppDbContext _context;

        public SalesReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SalesReport> GenerateDailyReportAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate)
                .ToListAsync();

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;
            var totalProductsSold = await _context.OrderItems
                .Where(oi => oi.Order.CreatedAt >= startDate && oi.Order.CreatedAt < endDate)
                .SumAsync(oi => oi.Quantity);
            var totalCustomers = orders.Select(o => o.UserId ?? 0).Distinct().Count();
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            return new SalesReport
            {
                ReportDate = date,
                Period = "Daily",
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalProductsSold = totalProductsSold,
                TotalCustomers = totalCustomers,
                AverageOrderValue = averageOrderValue,
                CreatedAt = DateTime.Now
            };
        }

        public async Task<SalesReport> GenerateWeeklyReportAsync(DateTime startDate)
        {
            var endDate = startDate.AddDays(7);

            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate)
                .ToListAsync();

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;
            var totalProductsSold = await _context.OrderItems
                .Where(oi => oi.Order.CreatedAt >= startDate && oi.Order.CreatedAt < endDate)
                .SumAsync(oi => oi.Quantity);
            var totalCustomers = orders.Select(o => o.UserId ?? 0).Distinct().Count();
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            return new SalesReport
            {
                ReportDate = startDate,
                Period = "Weekly",
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalProductsSold = totalProductsSold,
                TotalCustomers = totalCustomers,
                AverageOrderValue = averageOrderValue,
                CreatedAt = DateTime.Now
            };
        }

        public async Task<SalesReport> GenerateMonthlyReportAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate)
                .ToListAsync();

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;
            var totalProductsSold = await _context.OrderItems
                .Where(oi => oi.Order.CreatedAt >= startDate && oi.Order.CreatedAt < endDate)
                .SumAsync(oi => oi.Quantity);
            var totalCustomers = orders.Select(o => o.UserId ?? 0).Distinct().Count();
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            return new SalesReport
            {
                ReportDate = startDate,
                Period = "Monthly",
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalProductsSold = totalProductsSold,
                TotalCustomers = totalCustomers,
                AverageOrderValue = averageOrderValue,
                CreatedAt = DateTime.Now
            };
        }

        public async Task<SalesReport> GenerateYearlyReportAsync(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = startDate.AddYears(1);

            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate)
                .ToListAsync();

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;
            var totalProductsSold = await _context.OrderItems
                .Where(oi => oi.Order.CreatedAt >= startDate && oi.Order.CreatedAt < endDate)
                .SumAsync(oi => oi.Quantity);
            var totalCustomers = orders.Select(o => o.UserId ?? 0).Distinct().Count();
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            return new SalesReport
            {
                ReportDate = startDate,
                Period = "Yearly",
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalProductsSold = totalProductsSold,
                TotalCustomers = totalCustomers,
                AverageOrderValue = averageOrderValue,
                CreatedAt = DateTime.Now
            };
        }

        public async Task<IEnumerable<ProductSalesData>> GetTopSellingProductsAsync(int limit = 10)
        {
            return await _context.OrderItems
                .GroupBy(oi => new { oi.ProductId, oi.ProductName })
                .Select(g => new ProductSalesData
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Quantity * oi.Price),
                    Profit = g.Sum(oi => oi.Quantity * oi.Price) * 0.2m // Assuming 20% profit margin
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<CategorySalesData>> GetCategorySalesAsync()
        {
            var totalRevenue = await GetTotalRevenueAsync();
            
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .ThenInclude(p => p.Category)
                .GroupBy(oi => new { oi.Product.CategoryId, oi.Product.Category.Name })
                .Select(g => new CategorySalesData
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    ProductsSold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Quantity * oi.Price),
                    Percentage = totalRevenue > 0 ? (g.Sum(oi => oi.Quantity * oi.Price) / totalRevenue) * 100 : 0
                })
                .OrderByDescending(c => c.Revenue)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Orders.AsQueryable();
            
            if (startDate.HasValue)
                query = query.Where(o => o.CreatedAt >= startDate.Value);
            
            if (endDate.HasValue)
                query = query.Where(o => o.CreatedAt <= endDate.Value);

            return await query.SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetTotalOrdersAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Orders.AsQueryable();
            
            if (startDate.HasValue)
                query = query.Where(o => o.CreatedAt >= startDate.Value);
            
            if (endDate.HasValue)
                query = query.Where(o => o.CreatedAt <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task<int> GetTotalCustomersAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Users.AsQueryable();
            
            if (startDate.HasValue)
                query = query.Where(u => u.CreatedAt >= startDate.Value);
            
            if (endDate.HasValue)
                query = query.Where(u => u.CreatedAt <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task<decimal> GetAverageOrderValueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var totalRevenue = await GetTotalRevenueAsync(startDate, endDate);
            var totalOrders = await GetTotalOrdersAsync(startDate, endDate);
            
            return totalOrders > 0 ? totalRevenue / totalOrders : 0;
        }
    }
}

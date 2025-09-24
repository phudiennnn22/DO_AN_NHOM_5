using DOAN.Models;

namespace DOAN.Respository
{
    public interface ISalesReportService
    {
        Task<SalesReport> GenerateDailyReportAsync(DateTime date);
        Task<SalesReport> GenerateWeeklyReportAsync(DateTime startDate);
        Task<SalesReport> GenerateMonthlyReportAsync(int year, int month);
        Task<SalesReport> GenerateYearlyReportAsync(int year);
        Task<IEnumerable<ProductSalesData>> GetTopSellingProductsAsync(int limit = 10);
        Task<IEnumerable<CategorySalesData>> GetCategorySalesAsync();
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetTotalOrdersAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetTotalCustomersAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetAverageOrderValueAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}

using DOAN.Models;
using DOAN.Respository;
using DOAN.Service;
using DOAN.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace DOAN.Controllers
{
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISalesReportService _salesReportService;
        private readonly IPreOrderService _preOrderService;
        private readonly IProductReviewService _reviewService;

        public AdminController(
            IProductService productService,
            ICategoryService categoryService,
            ISalesReportService salesReportService,
            IPreOrderService preOrderService,
            IProductReviewService reviewService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _salesReportService = salesReportService;
            _preOrderService = preOrderService;
            _reviewService = reviewService;
        }

        public IActionResult Index()
        {
            // Check if user is admin
            if (!HttpContext.Session.IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang admin.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Product Management
        [HttpGet]
        public async Task<IActionResult> Products()
        {
            if (!HttpContext.Session.IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập.";
                return RedirectToAction("Index", "Home");
            }

            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Inventory()
        {
            if (!HttpContext.Session.IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập.";
                return RedirectToAction("Index", "Home");
            }

            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult SalesReports()
        {
            if (!HttpContext.Session.IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PreOrders()
        {
            if (!HttpContext.Session.IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập.";
                return RedirectToAction("Index", "Home");
            }

            var preOrders = await _preOrderService.GetAllPreOrdersAsync();
            return View(preOrders);
        }

        [HttpGet]
        public async Task<IActionResult> Reviews()
        {
            if (!HttpContext.Session.IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập.";
                return RedirectToAction("Index", "Home");
            }

            var reviews = await _reviewService.GetAllReviewsAsync();
            return View(reviews);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            if (!HttpContext.Session.IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập.";
                return RedirectToAction("Index", "Home");
            }

            // Load categories for dropdown
            ViewBag.Categories = await _categoryService.GetActiveCategoriesAsync();

            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product model)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Set default values
            model.IsActive = true;
            model.IsFeatured = false;
            if (model.SalePrice == null)
                model.SalePrice = 0;

            var success = await _productService.CreateProductAsync(model);
            
            if (success)
            {
                TempData["SuccessMessage"] = $"Tạo sản phẩm '{model.Name}' thành công!";
                return RedirectToAction("Products");
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể tạo sản phẩm. Vui lòng thử lại.";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập.";
                return RedirectToAction("Index", "Home");
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(Product model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditProduct", model);
            }

            model.UpdatedAt = DateTime.Now;
            var success = await _productService.UpdateProductAsync(model);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công.";
                return RedirectToAction("Products");
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật sản phẩm.";
                return View("EditProduct", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm.";
            }

            return RedirectToAction("Products");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(int productId, int newStock)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            product.Stock = newStock;
            product.UpdatedAt = DateTime.Now;
            
            var success = await _productService.UpdateProductAsync(product);
            if (success)
            {
                TempData["SuccessMessage"] = $"Cập nhật tồn kho sản phẩm {product.Name} thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật tồn kho.";
            }

            return RedirectToAction("Inventory");
        }


        [HttpGet]
        public async Task<IActionResult> GenerateReport(string period, DateTime? date = null)
        {
            SalesReport report = period.ToLower() switch
            {
                "daily" => await _salesReportService.GenerateDailyReportAsync(date ?? DateTime.Today),
                "weekly" => await _salesReportService.GenerateWeeklyReportAsync(date ?? DateTime.Today.AddDays(-7)),
                "monthly" => await _salesReportService.GenerateMonthlyReportAsync(
                    (date ?? DateTime.Today).Year, 
                    (date ?? DateTime.Today).Month),
                "yearly" => await _salesReportService.GenerateYearlyReportAsync((date ?? DateTime.Today).Year),
                _ => throw new ArgumentException("Invalid period")
            };

            return Json(report);
        }


        // Dashboard Statistics
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.Today;
            var thisMonth = DateTime.Today.AddDays(-30);

            var totalRevenue = await _salesReportService.GetTotalRevenueAsync();
            var totalOrders = await _salesReportService.GetTotalOrdersAsync();
            var totalCustomers = await _salesReportService.GetTotalCustomersAsync();
            var averageOrderValue = await _salesReportService.GetAverageOrderValueAsync();

            var monthlyRevenue = await _salesReportService.GetTotalRevenueAsync(thisMonth, DateTime.Today);
            var monthlyOrders = await _salesReportService.GetTotalOrdersAsync(thisMonth, DateTime.Today);

            var topProducts = await _salesReportService.GetTopSellingProductsAsync(5);
            var categorySales = await _salesReportService.GetCategorySalesAsync();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.AverageOrderValue = averageOrderValue;
            ViewBag.MonthlyRevenue = monthlyRevenue;
            ViewBag.MonthlyOrders = monthlyOrders;
            ViewBag.TopProducts = topProducts;
            ViewBag.CategorySales = categorySales;

            return View();
        }

        // Category Management
        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            if (!HttpContext.Session.IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập.";
                return RedirectToAction("Index", "Home");
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            if (!HttpContext.Session.IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập.";
                return RedirectToAction("Index", "Home");
            }

            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category model)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.CreatedAt = DateTime.Now;
            model.IsActive = true;

            var success = await _categoryService.CreateCategoryAsync(model);
            
            if (success)
            {
                TempData["SuccessMessage"] = $"Tạo danh mục '{model.Name}' thành công!";
                return RedirectToAction("Categories");
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể tạo danh mục. Vui lòng thử lại.";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            if (!HttpContext.Session.IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập.";
                return RedirectToAction("Index", "Home");
            }

            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(Category model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditCategory", model);
            }

            var success = await _categoryService.UpdateCategoryAsync(model);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Cập nhật danh mục thành công.";
                return RedirectToAction("Categories");
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật danh mục.";
                return View("EditCategory", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Xóa danh mục thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa danh mục.";
            }

            return RedirectToAction("Categories");
        }

    }
}

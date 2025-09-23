using DOAN.Data;
using DOAN.Respository;
using DOAN.Service; // namespace chứa CartService, ICartService, ChatService, IChatService, HomeService, IHomeService, ProductService, IProductService
using Microsoft.EntityFrameworkCore;

// Nếu bạn muốn xài API Chat riêng thì cần thêm namespace ChatBoxAPI.Services


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Cho MVC (Razor view)
builder.Services.AddControllersWithViews();

// Cho API
builder.Services.AddControllers();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

// Đăng ký các service của DOAN
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPreOrderService, PreOrderService>();
builder.Services.AddScoped<IProductReviewService, ProductReviewService>();
builder.Services.AddScoped<ISalesReportService, SalesReportService>();

// Đăng ký ChatService của API (HttpClient)
builder.Services.AddHttpClient<IChatService, ChatService>();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DataSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

app.UseSession();
app.UseAuthorization();

// Map MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map API controllers (Swagger endpoints)
app.MapControllers();

app.Run();

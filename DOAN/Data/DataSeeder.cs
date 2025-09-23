using DOAN.Models;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            // Seed admin user first
            await AdminSeeder.SeedAdminAsync(context);

            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Điện thoại", Description = "Điện thoại thông minh các hãng", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=300&h=200&fit=crop" },
                    new Category { Name = "Laptop", Description = "Máy tính xách tay gaming và văn phòng", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=300&h=200&fit=crop" },
                    new Category { Name = "Phụ kiện", Description = "Tai nghe, sạc, ốp lưng điện tử", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1606220945770-b5b6c2c55bf1?w=300&h=200&fit=crop" },
                    new Category { Name = "Đồng hồ", Description = "Đồng hồ thông minh và đồng hồ đeo tay", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?w=300&h=200&fit=crop" },
                    new Category { Name = "Tablet", Description = "Máy tính bảng và iPad", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=300&h=200&fit=crop" },
                    new Category { Name = "Gaming", Description = "Thiết bị gaming và phụ kiện", IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1493711662062-fa541adb3fc8?w=300&h=200&fit=crop" }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();

                var phoneCategoryId = categories.First(c => c.Name == "Điện thoại").Id;
                var laptopCategoryId = categories.First(c => c.Name == "Laptop").Id;
                var accessoryCategoryId = categories.First(c => c.Name == "Phụ kiện").Id;
                var watchCategoryId = categories.First(c => c.Name == "Đồng hồ").Id;
                var tabletCategoryId = categories.First(c => c.Name == "Tablet").Id;
                var gamingCategoryId = categories.First(c => c.Name == "Gaming").Id;

                var products = new List<Product>
                {
                    // Điện thoại
                    new Product { Name = "iPhone 15 Pro Max", Description = "Điện thoại iPhone 15 Pro Max 256GB với chip A17 Pro mạnh mẽ, camera 48MP", Price = 32990000, SalePrice = 30990000, Stock = 50, CategoryId = phoneCategoryId, ImageUrl = "https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=400&h=400&fit=crop&crop=center", Brand = "Apple", Color = "Titanium Natural", IsFeatured = true },
                    new Product { Name = "iPhone 15", Description = "Điện thoại iPhone 15 128GB với chip A16 Bionic, camera 48MP", Price = 22990000, SalePrice = 20990000, Stock = 80, CategoryId = phoneCategoryId, ImageUrl = "https://images.unsplash.com/photo-1592750475338-74b7b21085ab?w=400&h=400&fit=crop&crop=center", Brand = "Apple", Color = "Blue", IsFeatured = true },
                    new Product { Name = "Samsung Galaxy S24 Ultra", Description = "Điện thoại Samsung Galaxy S24 Ultra 256GB với camera 200MP, S Pen", Price = 32990000, SalePrice = 30990000, Stock = 30, CategoryId = phoneCategoryId, ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400&h=400&fit=crop&crop=center", Brand = "Samsung", Color = "Titanium Black", IsFeatured = true },
                    new Product { Name = "Samsung Galaxy S24", Description = "Điện thoại Samsung Galaxy S24 128GB với AI tích hợp", Price = 19990000, SalePrice = 17990000, Stock = 60, CategoryId = phoneCategoryId, ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400&h=400&fit=crop&crop=center", Brand = "Samsung", Color = "Cobalt Violet", IsFeatured = false },
                    new Product { Name = "Xiaomi 14 Pro", Description = "Điện thoại Xiaomi 14 Pro 256GB với camera Leica", Price = 18990000, SalePrice = 16990000, Stock = 40, CategoryId = phoneCategoryId, ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400&h=400&fit=crop&crop=center", Brand = "Xiaomi", Color = "Black", IsFeatured = false },
                    new Product { Name = "OPPO Find X7 Ultra", Description = "Điện thoại OPPO Find X7 Ultra 256GB với camera Hasselblad", Price = 24990000, SalePrice = 22990000, Stock = 25, CategoryId = phoneCategoryId, ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400&h=400&fit=crop&crop=center", Brand = "OPPO", Color = "Black", IsFeatured = false },

                    // Laptop
                    new Product { Name = "MacBook Pro M3 14\"", Description = "Laptop MacBook Pro 14 inch M3 chip với hiệu năng vượt trội, 16GB RAM", Price = 45990000, SalePrice = 42990000, Stock = 20, CategoryId = laptopCategoryId, ImageUrl = "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=400&h=400&fit=crop&crop=center", Brand = "Apple", Color = "Space Gray", IsFeatured = true },
                    new Product { Name = "MacBook Air M3 13\"", Description = "Laptop MacBook Air 13 inch M3 chip siêu mỏng nhẹ, 8GB RAM", Price = 29990000, SalePrice = 27990000, Stock = 35, CategoryId = laptopCategoryId, ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400&h=400&fit=crop&crop=center", Brand = "Apple", Color = "Midnight", IsFeatured = true },
                    new Product { Name = "Dell XPS 13", Description = "Laptop Dell XPS 13 Intel i7 với thiết kế cao cấp, 16GB RAM", Price = 32990000, SalePrice = 29990000, Stock = 25, CategoryId = laptopCategoryId, ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400&h=400&fit=crop&crop=center", Brand = "Dell", Color = "Platinum Silver", IsFeatured = false },
                    new Product { Name = "ASUS ROG Strix G15", Description = "Laptop gaming ASUS ROG Strix G15 RTX 4060, AMD Ryzen 7", Price = 25990000, SalePrice = 23990000, Stock = 15, CategoryId = laptopCategoryId, ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400&h=400&fit=crop&crop=center", Brand = "ASUS", Color = "Black", IsFeatured = false },
                    new Product { Name = "HP Pavilion 15", Description = "Laptop HP Pavilion 15 Intel i5, 8GB RAM, 512GB SSD", Price = 15990000, SalePrice = 13990000, Stock = 30, CategoryId = laptopCategoryId, ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400&h=400&fit=crop&crop=center", Brand = "HP", Color = "Natural Silver", IsFeatured = false },
                    new Product { Name = "Lenovo ThinkPad X1", Description = "Laptop Lenovo ThinkPad X1 Carbon Intel i7, 16GB RAM", Price = 35990000, SalePrice = 32990000, Stock = 18, CategoryId = laptopCategoryId, ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400&h=400&fit=crop&crop=center", Brand = "Lenovo", Color = "Black", IsFeatured = false },

                    // Tablet
                    new Product { Name = "iPad Pro 12.9\" M2", Description = "Máy tính bảng iPad Pro 12.9 inch M2 chip, 128GB", Price = 24990000, SalePrice = 22990000, Stock = 25, CategoryId = tabletCategoryId, ImageUrl = "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400&h=400&fit=crop&crop=center", Brand = "Apple", Color = "Space Gray", IsFeatured = true },
                    new Product { Name = "iPad Air 10.9\" M1", Description = "Máy tính bảng iPad Air 10.9 inch M1 chip, 64GB", Price = 14990000, SalePrice = 12990000, Stock = 40, CategoryId = tabletCategoryId, ImageUrl = "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400&h=400&fit=crop&crop=center", Brand = "Apple", Color = "Blue", IsFeatured = true },
                    new Product { Name = "Samsung Galaxy Tab S9", Description = "Máy tính bảng Samsung Galaxy Tab S9 11 inch, 128GB", Price = 18990000, SalePrice = 16990000, Stock = 20, CategoryId = tabletCategoryId, ImageUrl = "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400&h=400&fit=crop&crop=center", Brand = "Samsung", Color = "Graphite", IsFeatured = false },

                    // Phụ kiện
                    new Product { Name = "AirPods Pro 2", Description = "Tai nghe AirPods Pro thế hệ 2 với chống ồn chủ động", Price = 5990000, SalePrice = 5490000, Stock = 100, CategoryId = accessoryCategoryId, ImageUrl = "https://images.unsplash.com/photo-1606220945770-b5b6c2c55bf1?w=400&h=400&fit=crop&crop=center", Brand = "Apple", Color = "White", IsFeatured = true },
                    new Product { Name = "AirPods 3", Description = "Tai nghe AirPods thế hệ 3 với thiết kế mới", Price = 4990000, SalePrice = 4490000, Stock = 80, CategoryId = accessoryCategoryId, ImageUrl = "https://images.unsplash.com/photo-1606220945770-b5b6c2c55bf1?w=400&h=400&fit=crop&crop=center", Brand = "Apple", Color = "White", IsFeatured = false },
                    new Product { Name = "Samsung Galaxy Buds2 Pro", Description = "Tai nghe Samsung Galaxy Buds2 Pro với chống ồn", Price = 3990000, SalePrice = 3490000, Stock = 60, CategoryId = accessoryCategoryId, ImageUrl = "https://images.unsplash.com/photo-1606220945770-b5b6c2c55bf1?w=400&h=400&fit=crop&crop=center", Brand = "Samsung", Color = "White", IsFeatured = false },
                    new Product { Name = "Sony WH-1000XM5", Description = "Tai nghe Sony WH-1000XM5 chống ồn hàng đầu", Price = 6990000, SalePrice = 6490000, Stock = 30, CategoryId = accessoryCategoryId, ImageUrl = "https://images.unsplash.com/photo-1606220945770-b5b6c2c55bf1?w=400&h=400&fit=crop&crop=center", Brand = "Sony", Color = "Black", IsFeatured = true },
                    new Product { Name = "MagSafe Charger", Description = "Sạc không dây MagSafe cho iPhone", Price = 1290000, SalePrice = 1090000, Stock = 150, CategoryId = accessoryCategoryId, ImageUrl = "https://images.unsplash.com/photo-1606220945770-b5b6c2c55bf1?w=400&h=400&fit=crop&crop=center", Brand = "Apple", Color = "White", IsFeatured = false },
                    new Product { Name = "iPhone 15 Pro Case", Description = "Ốp lưng iPhone 15 Pro Silicon Case chính hãng", Price = 890000, SalePrice = 790000, Stock = 200, CategoryId = accessoryCategoryId, ImageUrl = "https://images.unsplash.com/photo-1606220945770-b5b6c2c55bf1?w=400&h=400&fit=crop&crop=center", Brand = "Apple", Color = "Midnight", IsFeatured = false },

                    // Đồng hồ
                    new Product { Name = "Apple Watch Series 9", Description = "Đồng hồ thông minh Apple Watch Series 9 với chip S9", Price = 8990000, SalePrice = 7990000, Stock = 40, CategoryId = watchCategoryId, ImageUrl = "https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?w=400&h=400&fit=crop&crop=center", Brand = "Apple", Color = "Pink", Size = "45mm", IsFeatured = true },
                    new Product { Name = "Apple Watch Ultra 2", Description = "Đồng hồ thông minh Apple Watch Ultra 2 cho thể thao", Price = 19990000, SalePrice = 18990000, Stock = 15, CategoryId = watchCategoryId, ImageUrl = "https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?w=400&h=400&fit=crop&crop=center", Brand = "Apple", Color = "Titanium", Size = "49mm", IsFeatured = true },
                    new Product { Name = "Samsung Galaxy Watch6", Description = "Đồng hồ thông minh Samsung Galaxy Watch6 44mm", Price = 5990000, SalePrice = 5490000, Stock = 35, CategoryId = watchCategoryId, ImageUrl = "https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?w=400&h=400&fit=crop&crop=center", Brand = "Samsung", Color = "Graphite", Size = "44mm", IsFeatured = false },
                    new Product { Name = "Garmin Fenix 7", Description = "Đồng hồ thể thao Garmin Fenix 7 GPS", Price = 12990000, SalePrice = 11990000, Stock = 20, CategoryId = watchCategoryId, ImageUrl = "https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?w=400&h=400&fit=crop&crop=center", Brand = "Garmin", Color = "Black", Size = "47mm", IsFeatured = false },

                    // Gaming
                    new Product { Name = "PlayStation 5", Description = "Máy chơi game PlayStation 5 với 825GB SSD", Price = 12990000, SalePrice = 11990000, Stock = 25, CategoryId = gamingCategoryId, ImageUrl = "https://images.unsplash.com/photo-1493711662062-fa541adb3fc8?w=400&h=400&fit=crop&crop=center", Brand = "Sony", Color = "White", IsFeatured = true },
                    new Product { Name = "Xbox Series X", Description = "Máy chơi game Xbox Series X với 1TB SSD", Price = 11990000, SalePrice = 10990000, Stock = 20, CategoryId = gamingCategoryId, ImageUrl = "https://images.unsplash.com/photo-1493711662062-fa541adb3fc8?w=400&h=400&fit=crop&crop=center", Brand = "Microsoft", Color = "Black", IsFeatured = true },
                    new Product { Name = "Nintendo Switch OLED", Description = "Máy chơi game Nintendo Switch OLED 64GB", Price = 7990000, SalePrice = 7490000, Stock = 30, CategoryId = gamingCategoryId, ImageUrl = "https://images.unsplash.com/photo-1493711662062-fa541adb3fc8?w=400&h=400&fit=crop&crop=center", Brand = "Nintendo", Color = "Neon Blue/Red", IsFeatured = false },
                    new Product { Name = "Steam Deck 512GB", Description = "Máy chơi game Steam Deck 512GB NVMe SSD", Price = 15990000, SalePrice = 14990000, Stock = 10, CategoryId = gamingCategoryId, ImageUrl = "https://images.unsplash.com/photo-1493711662062-fa541adb3fc8?w=400&h=400&fit=crop&crop=center", Brand = "Valve", Color = "Black", IsFeatured = false },
                    new Product { Name = "Razer DeathAdder V3", Description = "Chuột gaming Razer DeathAdder V3 Pro", Price = 2990000, SalePrice = 2690000, Stock = 50, CategoryId = gamingCategoryId, ImageUrl = "https://images.unsplash.com/photo-1493711662062-fa541adb3fc8?w=400&h=400&fit=crop&crop=center", Brand = "Razer", Color = "Black", IsFeatured = false },
                    new Product { Name = "Logitech G Pro X", Description = "Bàn phím gaming Logitech G Pro X Mechanical", Price = 3990000, SalePrice = 3590000, Stock = 40, CategoryId = gamingCategoryId, ImageUrl = "https://images.unsplash.com/photo-1493711662062-fa541adb3fc8?w=400&h=400&fit=crop&crop=center", Brand = "Logitech", Color = "Black", IsFeatured = false }
                };
                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }
} 
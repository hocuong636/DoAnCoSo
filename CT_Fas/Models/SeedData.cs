using Microsoft.EntityFrameworkCore;

namespace CT_Fas.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Kiểm tra xem đã có dữ liệu chưa
                if (context.Categories.Any() || context.Products.Any())
                {
                    return;
                }

                // Thêm danh mục
                var categories = new Category[]
                {
                    new Category { Name = "Áo thun nam", Description = "Các loại áo thun dành cho nam" },
                    new Category { Name = "Áo sơ mi nam", Description = "Các loại áo sơ mi dành cho nam" },
                    new Category { Name = "Quần jean nam", Description = "Các loại quần jean dành cho nam" },
                    new Category { Name = "Áo thun nữ", Description = "Các loại áo thun dành cho nữ" },
                    new Category { Name = "Váy đầm", Description = "Các loại váy đầm thời trang" }
                };

                context.Categories.AddRange(categories);
                context.SaveChanges();

                // Thêm sản phẩm
                var products = new Product[]
                {
                    new Product
                    {
                        Name = "Áo thun nam basic",
                        Price = 199000,
                        Description = "Áo thun nam form rộng basic",
                        StockQuantity = 50,
                        Brand = "CT Fashion",
                        Size = "M,L,XL",
                        CategoryId = categories[0].Id
                    },
                    new Product
                    {
                        Name = "Áo sơ mi trắng",
                        Price = 299000,
                        Description = "Áo sơ mi trắng công sở",
                        StockQuantity = 30,
                        Brand = "CT Fashion",
                        Size = "M,L,XL",
                        CategoryId = categories[1].Id
                    },
                    new Product
                    {
                        Name = "Quần jean skinny",
                        Price = 399000,
                        Description = "Quần jean nam ống ôm",
                        StockQuantity = 25,
                        Brand = "CT Fashion",
                        Size = "29,30,31,32",
                        CategoryId = categories[2].Id
                    },
                    new Product
                    {
                        Name = "Áo thun nữ crop top",
                        Price = 159000,
                        Description = "Áo thun nữ form crop top năng động",
                        StockQuantity = 0,
                        Brand = "CT Fashion",
                        Size = "S,M,L",
                        CategoryId = categories[3].Id
                    },
                    new Product
                    {
                        Name = "Váy liền thân",
                        Price = 459000,
                        Description = "Váy liền thân công sở thanh lịch",
                        StockQuantity = 5,
                        Brand = "CT Fashion",
                        Size = "S,M,L",
                        CategoryId = categories[4].Id
                    },
                    new Product
                    {
                        Name = "Quần jean baggy",
                        Price = 359000,
                        Description = "Quần jean nam ống rộng",
                        StockQuantity = 40,
                        Brand = "CT Fashion",
                        Size = "29,30,31,32",
                        CategoryId = categories[2].Id
                    }
                };

                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }
    }
}
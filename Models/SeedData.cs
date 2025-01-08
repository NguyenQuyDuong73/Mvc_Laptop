// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using MvcLaptop.Data;
// using System.Linq;
// namespace MvcLaptop.Models;

// public static class SeedData
// {
//     public static void Initialize(IServiceProvider serviceProvider)
//     {
//         using (var context = new MvcLaptopContext(
//             serviceProvider.GetRequiredService<
//                 DbContextOptions<MvcLaptopContext>>()))
//         {
//             // Kiểm tra nếu bảng Category chưa có dữ liệu
//             if (context.Category == null || !context.Category.Any())
//             {
//                 context.Category!.AddRange(new List<Category>
//                 {
//                     new Category { Name_Category = "Asus" },
//                     new Category { Name_Category = "Acer" },
//                     new Category { Name_Category = "Lenovo" }
//                 });
//                 context.SaveChanges();
//             }

//             // Kiểm tra nếu bảng Product chưa có dữ liệu
//             if (context.Product == null || !context.Product.Any())
//             {
//                 var asusCategory = context.Category.FirstOrDefault(c => c.Name_Category == "Asus")?.CategoryId ?? 0;
//                 var acerCategory = context.Category.FirstOrDefault(c => c.Name_Category == "Acer")?.CategoryId ?? 0;
//                 var lenovoCategory = context.Category.FirstOrDefault(c => c.Name_Category == "Lenovo")?.CategoryId ?? 0;

//                 context.Product!.AddRange(new List<Product>
//                 {
//                     new Product
//                     {
//                         Title = "Laptop Asus Vivobook 14 OLED",
//                         CategoryId = asusCategory,
//                         Description = "Asus Vivobook 14 OLED là dòng laptop...",
//                         Quantity = 100,
//                         Price = 25000000M,
//                         ProductImages = new List<ProductImage>
//                         {
//                             new ProductImage
//                             {
//                                 ImageUrl = "https://example.com/asus-main.jpg",
//                                 IsMainImage = true
//                             },
//                             new ProductImage
//                             {
//                                 ImageUrl = "https://example.com/asus-secondary.jpg",
//                                 IsMainImage = false
//                             }
//                         }
//                     },
//                     new Product
//                     {
//                         Title = "Laptop Acer Predator",
//                         CategoryId = acerCategory,
//                         Description = "Acer Predator là dòng laptop gaming...",
//                         Quantity = 50,
//                         Price = 30000000M,
//                         ProductImages = new List<ProductImage>
//                         {
//                             new ProductImage
//                             {
//                                 ImageUrl = "https://example.com/acer-main.jpg",
//                                 IsMainImage = true
//                             }
//                         }
//                     },
//                     new Product
//                     {
//                         Title = "Laptop Lenovo LOQ",
//                         CategoryId = lenovoCategory,
//                         Description = "Lenovo LOQ là dòng laptop thông minh...",
//                         Quantity = 75,
//                         Price = 20000000M,
//                         ProductImages = new List<ProductImage>
//                         {
//                             new ProductImage
//                             {
//                                 ImageUrl = "https://example.com/lenovo-main.jpg",
//                                 IsMainImage = true
//                             }
//                         }
//                     }
//                 });
//                 context.SaveChanges();
//             }
//         }
//     }
// }
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcLaptop.Data;
using System.Linq;

namespace MvcLaptop.Models
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MvcLaptopContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MvcLaptopContext>>()))
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

                // Add roles
                string[] roles = { "Admin", "Staff", "Guest" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Add Admin user
                var adminEmail = "admin@example.com";
                var adminUser = await context.Users!.FirstOrDefaultAsync(u => u.Email == adminEmail);

                if (adminUser == null)
                {
                    var admin = new User
                    {
                        UserName = "admin",
                        Email = adminEmail,
                        Password = "Admin@123", // Gán mật khẩu trực tiếp
                        Name = "Administrator",
                        DOB = DateTime.Now.AddYears(-30)
                    };

                    context.Users.Add(admin);
                    await context.SaveChangesAsync();

                    // Gán vai trò cho Admin
                    var userManagerAdmin = await userManager.FindByEmailAsync(adminEmail);
                    if (userManagerAdmin != null)
                    {
                        await userManager.AddToRoleAsync(userManagerAdmin, "Admin");
                    }
                }

                // Add Categories
                if (context.Category == null || !context.Category.Any())
                {
                    context.Category!.AddRange(new List<Category>
                    {
                        new Category { Name_Category = "Asus" },
                        new Category { Name_Category = "Acer" },
                        new Category { Name_Category = "Lenovo" }
                    });
                    await context.SaveChangesAsync();
                }

                // Add Products
                if (context.Product == null || !context.Product.Any())
                {
                    var asusCategory = context.Category.FirstOrDefault(c => c.Name_Category == "Asus")?.CategoryId ?? 0;
                    var acerCategory = context.Category.FirstOrDefault(c => c.Name_Category == "Acer")?.CategoryId ?? 0;
                    var lenovoCategory = context.Category.FirstOrDefault(c => c.Name_Category == "Lenovo")?.CategoryId ?? 0;

                    context.Product!.AddRange(new List<Product>
                    {
                        new Product
                        {
                            Title = "Laptop Asus Vivobook 14 OLED",
                            CategoryId = asusCategory,
                            Description = "Asus Vivobook 14 OLED là dòng laptop...",
                            Quantity = 100,
                            Price = 25000000M,
                            ProductImages = new List<ProductImage>
                            {
                                new ProductImage
                                {
                                    ImageUrl = "https://example.com/asus-main.jpg",
                                    IsMainImage = true
                                },
                                new ProductImage
                                {
                                    ImageUrl = "https://example.com/asus-secondary.jpg",
                                    IsMainImage = false
                                }
                            }
                        },
                        new Product
                        {
                            Title = "Laptop Acer Predator",
                            CategoryId = acerCategory,
                            Description = "Acer Predator là dòng laptop gaming...",
                            Quantity = 50,
                            Price = 30000000M,
                            ProductImages = new List<ProductImage>
                            {
                                new ProductImage
                                {
                                    ImageUrl = "https://example.com/acer-main.jpg",
                                    IsMainImage = true
                                }
                            }
                        },
                        new Product
                        {
                            Title = "Laptop Lenovo LOQ",
                            CategoryId = lenovoCategory,
                            Description = "Lenovo LOQ là dòng laptop thông minh...",
                            Quantity = 75,
                            Price = 20000000M,
                            ProductImages = new List<ProductImage>
                            {
                                new ProductImage
                                {
                                    ImageUrl = "https://example.com/lenovo-main.jpg",
                                    IsMainImage = true
                                }
                            }
                        }
                    });
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}

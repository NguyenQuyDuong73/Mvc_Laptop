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
                           serviceProvider.GetRequiredService<DbContextOptions<MvcLaptopContext>>()))
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

                // Danh sách vai trò
                string[] roles = { "Admin", "Staff", "Guest" };

                foreach (var roleName in roles)
                {
                    // Kiểm tra trong RoleManager
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        // Kiểm tra trùng lặp trực tiếp trong cơ sở dữ liệu
                        var existingRole = await context.Roles.AnyAsync(r => r.Name == roleName);
                        if (!existingRole)
                        {
                            var result = await roleManager.CreateAsync(new Role { Name = roleName });
                            if (result.Succeeded)
                            {
                                Console.WriteLine($"Vai trò {roleName} đã được tạo.");
                            }
                            else
                            {
                                Console.WriteLine($"Không thể tạo vai trò {roleName}:");
                                foreach (var error in result.Errors)
                                {
                                    Console.WriteLine($"- {error.Description}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Vai trò {roleName} đã tồn tại trong cơ sở dữ liệu.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Vai trò {roleName} đã tồn tại.");
                    }
                }

                // Tạo tài khoản Admin nếu chưa tồn tại
                var adminEmail = "admin@example.com";
                var adminPassword = "Admin@123";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    var admin = new User
                    {
                        UserName = "admin",
                        Email = adminEmail,
                        Name = "Administrator",
                        DOB = DateTime.Now.AddYears(-30),
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(admin, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                        Console.WriteLine("Tài khoản Admin đã được tạo và gán vai trò Admin.");
                    }
                    else
                    {
                        Console.WriteLine("Không thể tạo tài khoản Admin:");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"- {error.Description}");
                        }
                    }
                }
                else
                {
                    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        Console.WriteLine("Vai trò Admin đã được gán cho tài khoản Admin.");
                    }
                    else
                    {
                        Console.WriteLine("Tài khoản Admin đã có vai trò Admin.");
                    }
                }
                // Thêm danh mục sản phẩm nếu chưa tồn tại
                if (!context.Category!.Any())
                {
                    context.Category!.AddRange(new List<Category>
                    {
                        new Category { Name_Category = "Asus" },
                        new Category { Name_Category = "Acer" },
                        new Category { Name_Category = "Lenovo" }
                    });
                    await context.SaveChangesAsync();
                    Console.WriteLine("Danh mục sản phẩm đã được thêm.");
                }

                // Thêm sản phẩm nếu chưa tồn tại
                if (!context.Product.Any())
                {
                    var asusCategory = context.Category!.FirstOrDefault(c => c.Name_Category == "Asus")?.CategoryId ?? 0;
                    var acerCategory = context.Category!.FirstOrDefault(c => c.Name_Category == "Acer")?.CategoryId ?? 0;
                    var lenovoCategory = context.Category!.FirstOrDefault(c => c.Name_Category == "Lenovo")?.CategoryId ?? 0;

                    context.Product.AddRange(new List<Product>
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
                    Console.WriteLine("Sản phẩm đã được thêm.");
                }
            }
        }
    }
}

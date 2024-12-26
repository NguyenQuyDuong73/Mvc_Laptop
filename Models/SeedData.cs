using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcLaptop.Data;
using System;
using System.Linq;

namespace MvcLaptop.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new MvcLaptopContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<MvcLaptopContext>>()))
        {
            // Look for any Laptops.
            if (context.Laptop.Any())
            {
                return;   // DB has been seeded
            }
            context.Laptop.AddRange(
                new Laptop
                {
                    Title = "Laptop Asus Vivobook 14 OLED",
                    Genre = "Asus",
                    Description = "Asus Vivobook 14 OLED là một dòng laptop tầm trung, nổi bật với màn hình OLED chất lượng cao, hiệu năng ổn định, và thiết kế mỏng nhẹ. Máy hướng đến đối tượng người dùng văn phòng, sinh viên hoặc những ai cần một thiết bị nhỏ gọn, mạnh mẽ để xử lý các tác vụ hàng ngày và giải trí đa phương tiện.",
                    Price = 7.99M,
                    ImageUrl = "https://anphat.com.vn/media/product/44758_laptop_asus_vivobook_14_oled_a1405va_km095w__2_.jpg"
                },
                new Laptop
                {
                    Title = "Laptop Acer Predator",
                    Genre = "Acer",
                    Description = "Acer Predator PT515-51-731Z là một chiếc laptop gaming cao cấp thuộc dòng Predator Triton 500 của Acer. Máy được thiết kế cho game thủ chuyên nghiệp và người dùng yêu cầu hiệu năng cao, với cấu hình mạnh mẽ, màn hình tần số quét cao, và thiết kế mỏng nhẹ so với chuẩn laptop gaming.",
                    Price = 8.99M,
                    ImageUrl = "https://anphat.com.vn/media/product/30547_laptop_acer_predator_pt515_51_731z_nh_q4xsv_006_1.jpg"
                },
                new Laptop
                {
                    Title = "Laptop Lenovo LOQ",
                    Genre = "Lenovo",
                    Description = "Lenovo LOV-15 (15.6 inch FHD IPS) là một chiếc laptop thông minh và hiệu năng cao của Lenovo với màn hình IPS 15.6 inch, hiệu năng ổn định, và thiết kế mỏng nhẹ.",
                    Price = 3.99M,
                    ImageUrl = "https://anphat.com.vn/media/product/45246_laptop_lenovo_lov_15_15_6_ips_1000_q4wkv_003_1.jpg"
                }
            );
            context.SaveChanges();
        }
    }
}

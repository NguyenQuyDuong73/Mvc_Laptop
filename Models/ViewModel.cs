using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLaptop.Models;

public class LaptopViewModel
{
    public int Id { get; set; }

    [Display(Name = "Tiêu đề")]
    public string? Title { get; set; }

    [Display(Name = "Mã danh mục")]
    public int CategoryId { get; set; }

    [Display(Name = "Danh mục")]
    public string? Name_Category { get; set; }

    [Display(Name = "Mô tả")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Display(Name = "Số lượng")]
    public int Quantity { get; set; }

    [DataType(DataType.Currency)]
    [Display(Name = "Giá")]
    public decimal Price { get; set; }

    [NotMapped]
    [Display(Name = "Giá hiển thị")]
    public string FormattedPrice
    {
        get
        {
            return string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:C0}", Price);
        }
    }
    [Display(Name = "URL hình ảnh")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Hình ảnh chính")]
    public bool IsMainImage { get; set; }
}
public class CartProductViewModel
{
    [Display(Name = "Sản phẩm")]
    public LaptopViewModel Product { get; set; } = new LaptopViewModel();

    [Display(Name = "Số lượng")]
    public int Quantity { get; set; }
}
public class OrderViewModel
{
    public int Id { get; set; }

    [Display(Name = "Họ tên")]
    public string? FullName { get; set; }

    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    [Display(Name = "Phương thức thanh toán")]
    public string? PaymentMethod { get; set; }

    [Display(Name = "Ngày đặt hàng")]
    [DataType(DataType.Date)]
    public DateTime OrderDate { get; set; }

    [Display(Name = "Tổng giá trị")]
    [DataType(DataType.Currency)]
    public decimal TotalPrice { get; set; }

    [Display(Name = "Tên người dùng")]
    public string? UserName { get; set; }

    [Display(Name = "Email")]
    public string? Email { get; set; }
}
public class UserViewModel
{
    [Display(Name = "ID Người dùng")]
    public string? Id { get; set; }

    [Display(Name = "Tên Đăng nhập")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string? Password { get; set; }
    
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Full name")]
    public string? Name { get; set; }

    [Required]
    [Display(Name = "Birth Date")]
    [DataType(DataType.Date)]
    public DateTime DOB { get; set; }
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public string? Email { get; set; }

    [Display(Name = "Ngày tạo")]
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Số lượng đơn hàng")]
    public int OrderCount { get; set; }

    [Display(Name = "Danh sách vai trò")]
    public List<string> Roles { get; set; } = new List<string>();
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLaptop.Models;

public class LaptopRequest
{
    public int Id { get; set; }
    [StringLength(60, MinimumLength = 3, ErrorMessage = "Tiêu đề phải bằng 3 đến 60 ký tự")]
    [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
    [Display(Name = "Tiêu đề")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập thể loại")]
    [Display(Name = "Thể loại")]
    [StringLength(30, ErrorMessage = "Thể loại không vượt quá 30 ký tự")]
    public string? Genre { get; set; }

    [StringLength(500, MinimumLength = 10, ErrorMessage = "Mô tả phải từ 10 đến 200 ký tự")]
    [Required(ErrorMessage = "Vui lòng nhập mô tả")]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }
    
    [Range(1, 1000, ErrorMessage = "Số lượng phải nằm trong khoảng từ 1 đến 1000")]
    [Required(ErrorMessage = "Vui lòng nhập số lượng")]
    [Display(Name = "Số lượng")]
    public int Quantity { get; set; }
    
    [Range(1, 10000, ErrorMessage = "Giá phải bằng 1 đến 10000")]
    [DataType(DataType.Currency)]
    [Required(ErrorMessage = "Vui lòng nhập giá")]
    [Display(Name = "Giá")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập URL hình ảnh")]
    [Display(Name = "URL hình ảnh")]
    [Url(ErrorMessage = "Vui lòng nhập một URL hợp lệ")]
    [StringLength(500, ErrorMessage = "URL hình ảnh không được vượt quá 500 ký tự")]
    public string? ImageUrl { get; set; }
}

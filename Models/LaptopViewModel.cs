using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLaptop.Models;

public class LaptopViewModel
{
    public int Id { get; set; }

    [Display(Name = "Tiêu đề")]
    public string? Title { get; set; }

    [Display(Name = "Thể loại")]
    public string? Genre { get; set; }
    [Display(Name = "Mô tả")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }
    [Display(Name = "Số lượng")]
    public int Quantity { get; set; }
    
    [DataType(DataType.Currency)]
    [Display(Name = "Giá")]
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}

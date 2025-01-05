using System.ComponentModel.DataAnnotations;

namespace MvcLaptop.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string? Name_Category { get; set; }

    // Quan hệ 1-nhiều với Product
    public ICollection<Product>? Products { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLaptop.Models;

public class ProductImage
{
    public int Id { get; set; }

    public string? ImageUrl { get; set; }

    // Có phải ảnh chính không?
    public bool IsMainImage { get; set; }

    // Quan hệ với Product
    public int ProductId { get; set; }
    public Product? Product { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLaptop.Models;

public class Laptop
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Genre { get; set; }
    public string? Description { get; set; }
    
    [Range(1, 10000)]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}

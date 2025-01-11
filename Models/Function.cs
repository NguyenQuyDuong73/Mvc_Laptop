using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLaptop.Models
{
    public class Function
    {
        [Key]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? Id { get; set; }

        [MaxLength(200)]
        [Required]
        public string? Name { get; set; }

        [MaxLength(200)]
        [Required]
        public string? Url { get; set; }

        [Required]
        public int SortOrder { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? ParentId { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? Icon { get; set; }

        public ICollection<Permission>? Permissions { get; set; }
    }
}
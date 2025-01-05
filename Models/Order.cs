using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLaptop.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required]
        public string? PhoneNumber { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public string? PaymentMethod { get; set; }

        public DateTime OrderDate { get; set; }
        
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }
    // Thêm khóa ngoại liên kết đến User
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}

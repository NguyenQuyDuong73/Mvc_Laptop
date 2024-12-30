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

        // Thêm thông tin người dùng
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}

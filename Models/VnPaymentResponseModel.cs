using System.ComponentModel.DataAnnotations;
namespace MvcLaptop.Models
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string? PaymentMethod { get; set; }
        public string? OrderDescription { get; set; }
        public string? OrderId { get; set; }
        public string? PaymentId { get; set; }
        public string? TransactionId { get; set; }
        public string? Token { get; set; }
        public string? VNPayResponseCode { get; set; }
    }
    public class VnPaymentRequestModel : Order
    {
        public string? Description { get; set; }
        [Required(ErrorMessage = "Please enter the amount")]
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class VnPayConfig
    {
        public static string? vnp_Url { get; set; }
        public static string? vnp_Returnurl { get; set; }
        public static string? vnp_TmnCode { get; set; }
        public static string? vnp_HashSecret { get; set; }
        public static string? querydr { get; set; }
    }
}
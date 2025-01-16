using MvcLaptop.Models;
using MvcLaptop.Utils;
using MvcLaptop.Utils.ConfigOptions;
using Microsoft.Extensions.Options;
using MvcLaptop.Utils.ConfigOptions.VNPay;

namespace MvcLaptop.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
    public class VnPayService : IVnPayService
    {
        private readonly VnPayConfigOptions _options;

        public VnPayService(IOptions<VnPayConfigOptions> options)
        {
            _options = options.Value;
        }

        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnPay = new VnPayLibrary();
            vnPay.AddRequestData("vnp_Version", _options.Version!);
            vnPay.AddRequestData("vnp_Command", _options.Command!);
            vnPay.AddRequestData("vnp_TmnCode", _options.TmnCode!);
            // Chuyển đổi Amount từ decimal sang long và nhân với 100
            long amountInVnd = (long)(model.Amount * 100);
            // Ghi log để kiểm tra
            Console.WriteLine($"[VnPayService] Original Amount (decimal): {model.Amount}");
            Console.WriteLine($"[VnPayService] Converted Amount (long): {amountInVnd}");
            vnPay.AddRequestData("vnp_Amount", amountInVnd.ToString());


            // vnPay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());

            vnPay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", _options.CurrCode!);
            vnPay.AddRequestData("vnp_IpAddr", UtilityHelper.GetIpAddress(context));
            vnPay.AddRequestData("vnp_Locale", _options.Locale!);

            vnPay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đơn hàng: " + model.Id);
            vnPay.AddRequestData("vnp_OrderType", "other");
            vnPay.AddRequestData("vnp_ReturnUrl", _options.PaymentBackReturnUrl!);

            vnPay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl = vnPay.CreateRequestUrl(_options.BaseUrl!, _options.HashSecret!);
            // Ghi log URL
            Console.WriteLine($"[VnPayService] Generated Payment URL: {paymentUrl}");
            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnPay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                Console.WriteLine($"[VnPayService] {key}: {value}");
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnPay.AddResponseData(key, value.ToString());
                }
            }
            var vnp_orderId = Convert.ToInt64(vnPay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnPay.GetResponseData("vnp_TransactionNo"));
            // Lấy giá trị trực tiếp dưới dạng chuỗi
            // string? vnp_orderId = vnPay.GetResponseData("vnp_TxnRef");
            // string? vnp_TransactionId = vnPay.GetResponseData("vnp_TransactionNo");
            // string? vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            // string? vnp_ResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
            // string? vnp_OrderInfo = vnPay.GetResponseData("vnp_OrderInfo");
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnPay.GetResponseData("vnp_OrderInfo");
            Console.WriteLine($"[VnPayService] vnp_TxnRef: {vnp_orderId}");
            Console.WriteLine($"[VnPayService] vnp_TransactionNo: {vnp_TransactionId}");
            Console.WriteLine($"[VnPayService] vnp_ResponseCode: {vnp_ResponseCode}");
            Console.WriteLine($"[VnPayService] vnp_OrderInfo: {vnp_OrderInfo}");
            Console.WriteLine($"[VnPayService] vnp_SecureHash: {vnp_SecureHash}");
            bool checkSignature = vnPay.ValidateSignature(vnp_SecureHash!, _options.HashSecret!);
            Console.WriteLine($"[VnPayService] Signature validation result: {checkSignature}");
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VNPayResponseCode = vnp_ResponseCode
            };
        }
    }
}
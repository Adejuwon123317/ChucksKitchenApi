using ChucksKitchenApi.Data;
using ChucksKitchenApi.DTOS;
using ChucksKitchenApi.Entity;
using ChucksKitchenApi.Enums;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Text.Json;

namespace ChucksKitchenApi.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ChucksDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public PaymentService(ChucksDbContext context, HttpClient httpClient,IConfiguration config)
        {
            _context = context;
            _httpClient = httpClient;
            _config = config;
        }
        public async Task<PaymentResponseDTO> InitializePaymentAsync(int orderId, string appUserId)
        {
            var order = await _context.Orders
                     .Include(o => o.AppUser)
                     .FirstOrDefaultAsync(o => o.Id == orderId && o.AppUserId == appUserId);

            if (order == null)
                throw new Exception("Order not found or not yours.");

            var reference = $"CHK-{Guid.NewGuid().ToString().Substring(0, 8)}";

            var payment = new Payment
            {
                OrderId = order.Id,
                Amount = order.TotalAmount,
                Reference = reference,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var secretKey = _config["Paystack:SecretKey"];

            var frontendBaseUrl = _config["Frontend:BaseUrl"];
            var requestPayload = new
            {
                email = order.AppUser!.Email,
                amount = (int)(order.TotalAmount * 100),
                reference = reference,
                callback_url = $"{frontendBaseUrl}/payment-callback"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");

            var response = await _httpClient.PostAsync(
                "https://api.paystack.co/transaction/initialize",
                content
            );

            var responseString = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonDocument.Parse(responseString);

            var authorizationUrl = jsonResponse
                .RootElement
                .GetProperty("data")
                .GetProperty("authorization_url")
                .GetString();

            return new PaymentResponseDTO
            {
                AuthorizationUrl = authorizationUrl,
                Reference = reference
            };
        }
        public async Task<bool> VerifyPaymentAsync(string reference)
        {
            var secretKey = _config["Paystack:SecretKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add(
                "Authorization",
                $"Bearer {secretKey}"
            );

            var response = await _httpClient.GetAsync(
                $"https://api.paystack.co/transaction/verify/{reference}"
            );

            var responseString = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonDocument.Parse(responseString);

            var paymentStatus = jsonResponse
                .RootElement
                .GetProperty("data")
                .GetProperty("status")
                .GetString();

            var payment = await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Reference == reference);

            if (payment == null)
                return false;

            if (paymentStatus == "success")
            {
                payment.Status = PaymentStatus.Success;

                if (payment.Order != null)
                {
                    payment.Order.Status = OrderStatus.Confirmed;
                }

                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}

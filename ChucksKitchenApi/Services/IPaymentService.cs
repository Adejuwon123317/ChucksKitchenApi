using ChucksKitchenApi.DTOS;

namespace ChucksKitchenApi.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponseDTO> InitializePaymentAsync(int orderId, string appUserId);

        Task<bool> VerifyPaymentAsync(string reference);
    }
}

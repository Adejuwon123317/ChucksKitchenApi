namespace ChucksKitchenApi.DTOS
{
    public class PaymentResponseDTO
    {
        public string AuthorizationUrl { get; set; } = string.Empty;

        public string Reference { get; set; } = string.Empty;
    }
}

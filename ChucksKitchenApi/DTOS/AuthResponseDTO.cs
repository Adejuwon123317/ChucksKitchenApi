namespace ChucksKitchenApi.DTOS
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } 
        public DateTime ExpiresAt { get; set; }
    }
}

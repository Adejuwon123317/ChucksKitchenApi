namespace ChucksKitchenApi.DTOS
{
    public class OrderResponseDTO
    {
        public int Id { get; set; }

        public string Status { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }
        public string OrderType { get; set; } = string.Empty;

        public string? DeliveryAddress { get; set; }

        public List<OrderItemDTO> Items { get; set; } = new();
    }
}

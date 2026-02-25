namespace ChucksKitchenApi.DTOS
{
    public class OrderItemDTO
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal TotalPrice { get; set; }
    }
}

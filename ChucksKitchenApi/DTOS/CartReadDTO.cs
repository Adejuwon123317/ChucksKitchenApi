namespace ChucksKitchenApi.DTOS
{
    public class CartReadDTO
    {
        public int CartItemId { get; set; }
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}

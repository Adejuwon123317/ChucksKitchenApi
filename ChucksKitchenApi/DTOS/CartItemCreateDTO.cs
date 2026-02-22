namespace ChucksKitchenApi.DTOS
{
    public class CartItemCreateDTO
    {
        public int MenuId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}

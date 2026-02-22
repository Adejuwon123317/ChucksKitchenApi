namespace ChucksKitchenApi.Entity
{
    public class CartItem
    {
        public int Id { get; set; }
        // Link to Cart
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        //link to Menu
        public int MenuId { get; set; }
        public Menu Menu { get; set; }
        public int Quantity { get; set; } = 1;
    }
}

namespace ChucksKitchenApi.Entity
{
    public class Cart
    {
        public int Id { get; set; }

        // Link to AppUser
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        // Navigation for cart items
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}

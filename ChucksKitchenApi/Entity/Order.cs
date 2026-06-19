using ChucksKitchenApi.Enums;
using System.Globalization;

namespace ChucksKitchenApi.Entity
{
    public class Order
    {
        public int Id { get; set; }
        public String AppUserId { get; set; } = string.Empty;
        public AppUser? AppUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // NEW
        public OrderType OrderType { get; set; } = OrderType.Pickup;

        public string? DeliveryAddress { get; set; }
    }
}

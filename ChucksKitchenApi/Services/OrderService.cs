using ChucksKitchenApi.Data;
using ChucksKitchenApi.DTOS;
using ChucksKitchenApi.Entity;
using ChucksKitchenApi.Enums;
using Microsoft.EntityFrameworkCore;

namespace ChucksKitchenApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly ChucksDbContext _context;
        public OrderService(ChucksDbContext context)
        {
            _context = context;
        }
        public async Task<OrderResponseDTO> CreateOrderAsync(string appUserId, CreateOrderDTO createOrderDTO)
        {
           var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Menu)
                .FirstOrDefaultAsync(c => c.AppUserId == appUserId);

            if (cart == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Cannot create order: Cart is empty.");

            var order = new Order
            {
                AppUserId = appUserId,
                Status = OrderStatus.Pending,
            };
            if (string.IsNullOrWhiteSpace(createOrderDTO.DeliveryAddress))
            {
                order.OrderType = OrderType.Pickup;
            }
            else
            {
                order.OrderType = OrderType.Delivery;
                order.DeliveryAddress = createOrderDTO.DeliveryAddress;
            }
            foreach (var item in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    MenuId = item.MenuId,
                    MenuName = item.Menu?.Name ?? string.Empty,
                    Price = item.Menu?.Price ?? 0,
                    Quantity = item.Quantity,
                    TotalPrice = (item.Menu?.Price ?? 0) * item.Quantity
                };

                order.TotalAmount += orderItem.TotalPrice;
                order.OrderItems.Add(orderItem);
            }
            _context.Orders.Add(order);

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();
            return await MapOrder(order.Id);

        }

        public async Task<OrderResponseDTO?> GetOrderByIdAsync(int orderId, string appUserId, bool isAdmin)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && (isAdmin || o.AppUserId == appUserId));
            if (order == null) return null;
            return await MapOrder(orderId);
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetUserOrdersAsync(string appUserId)
        {
            var orders  = await _context.Orders
                .Where(o => o.AppUserId == appUserId)
                .Select(o => o.Id)
                .ToListAsync();
            var result = new List<OrderResponseDTO>();
            foreach (var id in orders)
            {
                var orderDTO = await MapOrder(id);
                result.Add(orderDTO);
            }
            return result;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CancelOrderAsync(int orderId, string appUserId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.AppUserId == appUserId);
            if (order == null) return false;

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            return true;
        }
        private async Task<OrderResponseDTO> MapOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                throw new Exception("Order not found.");
            return new OrderResponseDTO
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                OrderType = order.OrderType.ToString(),
                DeliveryAddress = order.DeliveryAddress,

                Items = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    MenuId = oi.MenuId,
                    MenuName = oi.MenuName,
                    Price = oi.Price,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.TotalPrice
                }).ToList()
            };
        }
    }
}

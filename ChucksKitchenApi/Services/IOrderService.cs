using ChucksKitchenApi.DTOS;
using ChucksKitchenApi.Enums;

namespace ChucksKitchenApi.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDTO> CreateOrderAsync(string appUserId,CreateOrderDTO createOrderDTO);

        Task<OrderResponseDTO?> GetOrderByIdAsync(int orderId, string appUserId, bool isAdmin);

        Task<IEnumerable<OrderResponseDTO>> GetUserOrdersAsync(string appUserId);

        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);

        Task<bool> CancelOrderAsync(int orderId, string appUserId);
    }
}

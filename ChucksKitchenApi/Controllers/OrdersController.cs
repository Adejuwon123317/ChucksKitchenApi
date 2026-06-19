using ChucksKitchenApi.DTOS;
using ChucksKitchenApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChucksKitchenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO createOrderDTO)
        {
            try
            {
                var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _orderService.CreateOrderAsync(appUserId, createOrderDTO);
                return Ok(result); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            } 
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var result = await _orderService.GetOrderByIdAsync(id, appUserId, isAdmin);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetUserOrders()
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _orderService.GetUserOrdersAsync(appUserId);
            return Ok(result);
        }
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, PutOrderStatusDTO putDTO)
        {
            var updated = await _orderService.UpdateOrderStatusAsync(id, putDTO.Status);    
            if (!updated) return NotFound();
            return Ok("Order status updated.");
        }
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var canceled = await _orderService.CancelOrderAsync(id, appUserId);
            if (!canceled) return NotFound();
            return Ok("Order canceled.");
        }
    }
}

using ChucksKitchenApi.Data;
using ChucksKitchenApi.DTOS;
using ChucksKitchenApi.Entity;
using ChucksKitchenApi.Extensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChucksKitchenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ChucksDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IValidator<CartItemCreateDTO> _createValidator;
        private readonly IValidator<CartItemPutDTO> _putValidator;
        public CartsController(ChucksDbContext context, UserManager<AppUser> userManager, IValidator<CartItemCreateDTO> createValidator, IValidator<CartItemPutDTO> putValidator)

        {
            _context = context;
            _userManager = userManager;
            _createValidator = createValidator;
            _putValidator = putValidator;
        }
        private async Task<Cart> GetUserCart()
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Menu)
                .FirstOrDefaultAsync(c => c.AppUserId == userId);

            if (cart == null)
            {
                cart = new Cart { AppUserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            return cart;
        }
        //Get:/api/Carts
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCart()
        {
            var cart = await GetUserCart();

            var cartReadDtoList = cart.CartItems.Select(ci => new CartReadDTO
            {
                CartItemId = ci.Id,
                MenuId = ci.MenuId,
                MenuName = ci.Menu?.Name ?? string.Empty,
                Price = ci.Menu?.Price ?? 0m,
                Quantity = ci.Quantity,
                TotalPrice = (ci.Menu?.Price ?? 0m) * ci.Quantity,
                ImageUrl = ci.Menu?.ImageUrl ?? string.Empty
            }).ToList();

            return Ok(cartReadDtoList);
        }
        //post:/api/Carts
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddToCart(CartItemCreateDTO createDTO)
        {
            ValidationResult validationResult = await _createValidator.ValidateAsync(createDTO);

            //check the validation
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }
            var cart = await GetUserCart();
            var menu = await _context.Menus.FindAsync(createDTO.MenuId);

            if (menu == null)
                return NotFound("Menu not found.");

            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.MenuId == createDTO.MenuId);

            if (existingItem != null)
            {
                existingItem.Quantity += createDTO.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    MenuId = createDTO.MenuId,
                    Quantity = createDTO.Quantity
                };
                _context.CartItems.Add(cartItem);
            }
            await _context.SaveChangesAsync();
            return Ok("Item added to cart.");
        }
        //put:/api/Carts/{menuId}
        [HttpPut("{menuId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCart(int menuId, CartItemPutDTO putDTO)
        {
            ValidationResult validationResult = await _putValidator.ValidateAsync(putDTO);

            //check the validation
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            if (putDTO.Quantity <= 0)
                return BadRequest("Quantity must be greater than Zero");

            var cart = await GetUserCart();
            var item = cart.CartItems.FirstOrDefault(ci => ci.MenuId == menuId);

            if (item == null)
                return NotFound("Item not found in cart.");
            item.Quantity = putDTO.Quantity;

            await _context.SaveChangesAsync();
            return Ok("Cart updated.");
        }
        //delete:/api/Carts/{menuId}
        [HttpDelete("{menuId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveItem(int menuId)
        {
            var cart = await GetUserCart();

            var item = cart.CartItems.FirstOrDefault(ci => ci.MenuId == menuId);

            if (item == null)
                return NotFound("Item not found in cart.");

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("Item removed from cart.");
        }
    }
}

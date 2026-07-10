using ChucksKitchenApi.Data;
using ChucksKitchenApi.DTOS;
using ChucksKitchenApi.Entity;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ChucksKitchenApi.Extensions;
using Microsoft.AspNetCore.Authorization;


namespace ChucksKitchenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController : ControllerBase
    {
        private readonly ChucksDbContext _context;
        private readonly IValidator<MenuCreateDTO> _createValidator;
        private readonly IValidator<MenuPutDTO> _putValidator;
        public MenusController(ChucksDbContext context, IValidator<MenuCreateDTO> createValidator, IValidator<MenuPutDTO> putValidator)
        {
            _context = context;
            _createValidator = createValidator;
            _putValidator = putValidator;
        }
        //Get:/api/Menus
        [HttpGet]
        public async Task<IActionResult> GetMenus()
        {
            var menus = await _context.Menus.ToListAsync();
            var menuReadDtoList = menus.Select(x => new MenuReadDTO
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                IsAvailable = x.IsAvailable,
                CategoryId = x.CategoryId
            })
            .ToList();
            return Ok(menuReadDtoList);
        }
        //Post:/api/Menus
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMenu(MenuCreateDTO createDTO)
        {
            // firstly we validate
            ValidationResult validationResult = await _createValidator.ValidateAsync(createDTO);

            //check the validation
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            var menu = new Menu
            {
                Name = createDTO.Name,
                Price = createDTO.Price,
                Description = createDTO.Description,
                ImageUrl = createDTO.ImageUrl,
                IsAvailable = createDTO.IsAvailable,
                CategoryId = createDTO.CategoryId
            };
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMenu", new { id = menu.Id }, menu);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            return Ok(menu);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return NoContent();

        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMenu(int id ,MenuPutDTO putDTO)
        {
            // firstly we validate
            ValidationResult validationResult = await _putValidator.ValidateAsync(putDTO);

            //check the validation
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }
            var menu = new Menu
            {
                Id = id,
                Name = putDTO.Name,
                Price = putDTO.Price,
                Description = putDTO.Description,
                ImageUrl = putDTO.ImageUrl,
                IsAvailable = putDTO.IsAvailable,
                CategoryId = putDTO.CategoryId
            };
            if(id != menu.Id)
            {
                return BadRequest();
            }
            _context.Entry(menu).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)

            {
                if (!_context.Menus.Any(x => x.Id == id))
                {
                    return NotFound();
                }
                throw;

            }
            return NoContent();
            }
    }
}

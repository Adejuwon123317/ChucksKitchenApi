using ChucksKitchenApi.Data;
using ChucksKitchenApi.DTOS;
using ChucksKitchenApi.Entity;
using ChucksKitchenApi.Extensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ChucksKitchenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ChucksDbContext _context;
        private readonly IValidator<CategoryCreateDTO> _createValidator;
        private readonly IValidator<CategoryPutDTO> _putValidator;
        public CategoriesController(ChucksDbContext context, IValidator<CategoryCreateDTO> createValidator, IValidator<CategoryPutDTO> putValidator)
        {
            _context = context;
            _createValidator = createValidator;
            _putValidator = putValidator;
        }
        //Get:/api/Categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.Include(c => c.Menus).ToListAsync();
            var categoryReadDtoList = categories.Select(x => new CategoryReadDTO
            {
                Name = x.Name,
                Description = x.Description,
                Menus = x.Menus.Select(m => new MenuReadDTO
                {
                    Name = m.Name,
                    Price = m.Price,
                    Description = m.Description,
                    ImageUrl = m.ImageUrl,
                    IsAvailable = m.IsAvailable,
                }).ToList()
            })
            .ToList();
            return Ok(categoryReadDtoList);
        }
        //Get:/api/Category/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Menus)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            var categoryReadDto = new CategoryReadDTO
            {
                Name = category.Name,
                Description = category.Description,
                Menus = category.Menus.Select(m => new MenuReadDTO
                {
                    Name = m.Name,
                    Price = m.Price,
                    Description = m.Description,
                    ImageUrl = m.ImageUrl,
                    IsAvailable = m.IsAvailable
                }).ToList()
            };

            return Ok(categoryReadDto);
        }

        // Post:/api/Category
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryCreateDTO createDTO)
        {
            // firstly we validate
            ValidationResult validationResult = await _createValidator.ValidateAsync(createDTO);

            //check the validation
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            var category = new Category
            {
                Name = createDTO.Name,
                Description = createDTO.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }
        // Put:/api/Category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryPutDTO putDTO)
        {
            // firstly we validate
            ValidationResult validationResult = await _putValidator.ValidateAsync(putDTO);

            //check the validation
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }
            var category = new Category
            {
                Id = id,
                Name = putDTO.Name,
                Description = putDTO.Description
            };
            if (id != category.Id)
            {
                return BadRequest();
            }
            _context.Entry(category).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)

            {
                if (!_context.Categories.Any(x => x.Id == id))
                {
                    return NotFound();
                }
                throw;

            }
            return NoContent();
        }
        // Delete:/api/Category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}

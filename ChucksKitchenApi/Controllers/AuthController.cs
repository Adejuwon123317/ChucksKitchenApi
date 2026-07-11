using ChucksKitchenApi.DTOS;
using ChucksKitchenApi.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChucksKitchenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthController(UserManager<AppUser> userManager, IConfiguration config, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            // Firstly we loop the user in the database using usermanager email
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                return Unauthorized("Invalid email or password");
            }
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            //claims are pieces of ones unique identity like nin and bvn
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!),

            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));


            var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
            int.Parse(_config["Jwt:ExpiresInMinutes"]!)),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );


            return Ok(new AuthResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = token.ValidTo
            });
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO DTO)
        {
            //check if user already exists
            var userexists = await _userManager.FindByEmailAsync(DTO.EmailAddress);
            if (userexists != null)
            {
                return BadRequest("The user already exists");
            }

            var user = new AppUser
            {
                Email = DTO.EmailAddress,
                FirstName = DTO.FirstName,
                LastName = DTO.LastName,
                Nationality = DTO.Nationality,
                Gender = DTO.Gender,
                UserName = DTO.EmailAddress,
            };
            var result = await _userManager.CreateAsync(user, DTO.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            await _userManager.AddToRoleAsync(user, "Customer");
            return Ok("User registered successfully");
        }
    }
}

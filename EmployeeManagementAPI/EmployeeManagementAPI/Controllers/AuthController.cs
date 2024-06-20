using EmployeeManagementAPI.Models;
using EmployeeManagementAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace EmployeeManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AuthController(UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager,
                            ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }


        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if email already exists in the AspNetUsers table
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest("User already exists with the provided email.");
            }

            // Create a new ApplicationUser for authentication
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Phone,
                Department = model.Department,
            };

            var createUserResult = await _userManager.CreateAsync(user, model.Password);
            if (!createUserResult.Succeeded)
            {
                return BadRequest(createUserResult.Errors);
            }

            // Save the user ID for later use
            var userId = user.Id;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registered Successfully" });
        }



        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
                return Unauthorized("Invalid password.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email)
            };

            string jwtKey = JwtKeyGenerator.GenerateJwtKey();
            var jwtIssuer = Environment.GetEnvironmentVariable("Jwt_Issuer");
            var jwtAudience = Environment.GetEnvironmentVariable("Jwt_Audience");
            var jwtExpireDays = int.Parse(Environment.GetEnvironmentVariable("Jwt_ExpireDays"));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddDays(jwtExpireDays),
                signingCredentials: creds);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }


    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Database;
using UserManagement.Database.Models;
using UserManagement.JWTAuthentication;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        private readonly UserManagementContext _dbcontext;

        private readonly UserManager<ApplicationUser > _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IConfiguration _configuration;

            public UserController(ILogger<UserController> logger, 
                UserManagementContext dbcontext, 
                UserManager<ApplicationUser > userManager, 
                RoleManager<IdentityRole> roleManager, 
                IConfiguration configuration)
        {
            _logger = logger;
            _dbcontext = dbcontext;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpGet]
        public ApplicationUser  Get()
        {
            ApplicationUser  user = new();
            user.Firstname = HttpContext.User.Identity.Name;

            return user;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Search the User table for the users email address.
            ApplicationUser  user = await _userManager.FindByEmailAsync(email.ToLower());

            // If a user with this email isn't found then return Unauthorized.
            if (user == null)
                return Unauthorized();

            // Check the password.
            bool result = await _userManager.CheckPasswordAsync(user, password);

            // If the password fails verification then return Unauthorized.
            if (result == false)
                return Unauthorized();

            // Create a security key to sign the token with.
            SymmetricSecurityKey authSigningKey = new(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Get the users roles and add them to the token.
            var userRoles = await _userManager.GetRolesAsync(user);
            authClaims.AddRange(userRoles.Select(x => new Claim(ClaimTypes.Role, x)));

            // Create the token
            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            // Send the token back to complete the login.
            return Ok(token);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email.ToLower());

            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            // Create a new user.
            ApplicationUser  user = new()
            {
                Email = model.Email,
                Firstname = model.Firstname,
                Middlename = model.Middlename,
                Lastname = model.Lastname,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                string message = "User creation failed! Please check user details and try again.";

                if(result.Errors.Any())
                    message = result.Errors.First().Description;

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = message });
            }

            // Add user role.
            await _userManager.AddToRoleAsync(user, UserRoles.User);

            // Get and return the login token if registration is successful. This saves a user needing to log in directly after registering (which is old-hat).
            return await Login(model.Email, model.Password);
        }
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
using UserManagement.Models;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        private readonly UserManagementContext _dbcontext;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IConfiguration _configuration;

        private readonly Mapper _mapper;

        public UsersController(ILogger<UsersController> logger,
                UserManagementContext dbcontext,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                IConfiguration configuration)
        {
            _logger = logger;
            _dbcontext = dbcontext;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;

            // Create an instance of AutoMapper to make it easier to clean models for data transfer.
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ApplicationUser, User>());
            _mapper = new Mapper(config);
        }

        [Authorize(Roles = UserRoles.StaffOrAdmin)]
        [HttpGet]
        [Route("/Users")]
        public List<User> Users()
        {
            var users = (List<User>)_userManager.Users.Select(x => _mapper.Map<User>(x));
            return users;
        }

        [Authorize]
        [HttpGet]
        [Route("/Users/{userId}")]
        public User GetUser(string UserId = null)
        {
            ApplicationUser user;
            var currentUserId = _userManager.GetUserId(this.User);

            if (UserId != null && UserId != currentUserId)
            {
                if (!this.User.IsInRole(UserRoles.Admin) && !this.User.IsInRole(UserRoles.Staff))
                    return null;

                user = _userManager.FindByIdAsync(UserId).Result;
            }
            else
            {
                user = _userManager.GetUserAsync(this.User).Result;
            }

            return _mapper.Map<User>(user);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            if (model == null)
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Bad Request", Message = "Request body was badly formatted or empty." });

            return await Login2(model.Email, model.Password);
        }

        private async Task<IActionResult> Login2(string email, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Bad Request", Message = "Email is missing." });

                if (string.IsNullOrWhiteSpace(password))
                    return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Bad Request", Message = "Password is mission." });

                // Search the User table for the users email address.
                ApplicationUser user = await _userManager.FindByEmailAsync(email.ToLower());

                // If a user with this email isn't found then return Unauthorized.
                if (user == null)
                    return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Unauthorized", Message = "Email or password is incorrect." });

                // Check the password.
                bool result = await _userManager.CheckPasswordAsync(user, password);

                // If the password fails verification then return Unauthorized.
                if (result == false)
                    return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Unauthorized", Message = "Email or password is incorrect." });

                // Create a security key to sign the token with.
                SymmetricSecurityKey authSigningKey = new(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var authClaims = new List<Claim> {
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

                // Send back an authentication token.
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occured while trying to process a user login", email);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Login failed with an unknown exception." });
            }
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] User model)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(model.Email.ToLower());

                if (userExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

                // Create a new user.
                ApplicationUser user = new()
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

                    if (result.Errors.Any())
                        message = result.Errors.First().Description;

                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = message });
                }

                // Add user role.
                await _userManager.AddToRoleAsync(user, UserRoles.User);

                // Get and return the login token if registration is successful. This saves a user needing to log in directly after registering (which is old-hat).
                return await Login2(model.Email, model.Password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occured while trying to process a user registration", model);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed with an unknown exception." });
            }
        }

        [HttpPut]
        [Route("/Users")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<User> UpdateUser([FromBody] User user)
        {
            // Find the user with matching id.
            ApplicationUser appUser = await _userManager.FindByIdAsync(user.Id);

            // Set the fields which can be changed.
            appUser.Email = user.Email;
            appUser.Firstname = user.Firstname;
            appUser.Middlename = user.Middlename;
            appUser.Lastname = user.Lastname;

            // Update the user.
            await _userManager.UpdateAsync(appUser);

            // Return the values, as saved, back to the caller.
            return _mapper.Map<User>(appUser);
        }
    }
}
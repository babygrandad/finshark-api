using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account; // Importing necessary DTOs for account actions
using api.interfaces; // Importing custom interfaces
using api.Models; // Importing custom models
using Microsoft.AspNetCore.Identity; // Importing Identity for user management
using Microsoft.AspNetCore.Mvc; // Importing MVC for creating controllers and actions
using Microsoft.EntityFrameworkCore; // Importing Entity Framework Core for database operations
using Microsoft.Identity.Client; // Importing Identity Client for token management (not used in the code)

namespace api.Controllers
{
    [Route("api/account")] // Defining the route for this controller
    [ApiController] // Indicating this is an API controller with automatic model validation
    public class AccountController : ControllerBase
    {
        // Fields for managing dependencies, which are injected via the constructor
        private readonly UserManager<AppUser> _userManager; // Manages user creation, deletion, and queries
        private readonly ITokenService _tokenService; // Custom service for creating tokens
        private readonly SignInManager<AppUser> _signInManager; // Manages user sign-in process

        // Constructor for injecting dependencies
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager; // Initializing UserManager
            _tokenService = tokenService; // Initializing TokenService
            _signInManager = signInManager; // Initializing SignInManager
        }

        // POST endpoint for user login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid) // Check if the model state is valid
            {
                return BadRequest(ModelState); // Return 400 Bad Request if model state is invalid
            }

            // Find user by username (converted to lowercase)
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

            if (user == null) // Check if user exists
            {
                return Unauthorized("Invalid username or password."); // Return 401 Unauthorized if user not found
            }

            // Check if the password is correct
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) // Check if the sign-in was successful
            {
                return Unauthorized("Invalid username or password."); // Return 401 Unauthorized if password is incorrect
            }

            // Return user data and token if login is successful
            return Ok(
                new UserDto
                {
                    UserName = user.UserName, // Set username
                    EmailAddress = user.Email, // Set email address
                    Token = _tokenService.CreateToken(user) // Generate and set token
                }
            );
        }

        // POST endpoint for user registration
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto) // Binding the request body to RegisterDto
        {
            try
            {
                if (!ModelState.IsValid) // Check if the model state is valid
                {
                    return BadRequest(ModelState); // Return 400 Bad Request if model state is invalid
                }

                // Create a new AppUser object with the provided username and email
                var appUser = new AppUser
                {
                    UserName = registerDto.UserName, // Set username
                    Email = registerDto.EmailAddress, // Set email address
                };

                // Create the user with the provided password
                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded) // Check if user creation was successful
                {
                    // Add the user to the "User" role
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded) // Check if role assignment was successful
                    {
                        // Return user data and token if registration is successful
                        return Ok(
                            new UserDto
                            {
                                UserName = appUser.UserName, // Set username
                                EmailAddress = appUser.Email, // Set email address
                                Token = _tokenService.CreateToken(appUser) // Generate and set token
                            }
                        );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors); // Return 500 Internal Server Error if role assignment failed
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors); // Return 500 Internal Server Error if user creation failed
                }
            }
            catch (Exception e) // Catch any exceptions that occur during the process
            {
                return StatusCode(500, e); // Return 500 Internal Server Error with the exception details
            }
        }
    }
}

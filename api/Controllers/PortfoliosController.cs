using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using api.Data;
using api.Extensions;
using api.interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("/api/portfolio")]
    [ApiController] // Indicates that this is an API controller, which provides automatic model validation and other features
    public class PortfoliosController : ControllerBase
    {
        // These fields store instances of the respective repositories and UserManager
        // These instances will be provided by dependency injection
        private readonly ICommentRepository _commentRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;
        private readonly IPortfolioRepository _portfolioRepo;

        // Constructor that takes instances of UserManager, IStockRepository, ICommentRepository, and IPortfolioRepository
        // These instances are injected by the dependency injection system
        public PortfoliosController(UserManager<AppUser> userManager,
        IStockRepository stockRepo,
        ICommentRepository commentRepo,
        IPortfolioRepository portfolioRepo)
        {
            _userManager = userManager; // Assign the injected UserManager to the private field
            _stockRepo = stockRepo; // Assign the injected IStockRepository to the private field
            _commentRepo = commentRepo; // Assign the injected ICommentRepository to the private field
            _portfolioRepo = portfolioRepo; // Assign the injected IPortfolioRepository to the private field
        }

        // HTTP GET endpoint to get the portfolio of the authenticated user
        // Requires the user to be authorized (authenticated)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUsername(); // Retrieve the username from the claims (via an extension method in api.Extensions)
            var AppUser = await _userManager.FindByNameAsync(username); // Find the AppUser object in the database using the username
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(AppUser); // Get the portfolio of the user from the IPortfolioRepository
            return Ok(userPortfolio); // Return the user's portfolio with an HTTP 200 OK status
        }

        // HTTP POST endpoint to add a new stock to the user's portfolio
        // Requires the user to be authorized (authenticated)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.GetUsername(); // Retrieve the username from the claims (via an extension method in api.Extensions)
            var AppUser = await _userManager.FindByNameAsync(username); // Find the AppUser object in the database using the username

            var stock = await _stockRepo.GetBySymbolAsync(symbol); // Try to fetch the stock from the database using its symbol

            if (stock == null) // Check if the stock was found
            {
                return BadRequest("Stock not found"); // If the stock was not found, return a 400 Bad Request response with an error message
            }

            var userPortfolio = await _portfolioRepo.GetUserPortfolio(AppUser); // Fetch the user's existing portfolio records from the IPortfolioRepository

            if (userPortfolio.Any(e => e.Symbol.ToLower() == symbol.ToLower())) // Check if the stock is already in the user's portfolio (case insensitive)
            {
                return BadRequest("The stock already exists in your portfolio"); // If the stock is already in the portfolio, return a 400 Bad Request response with an error message
            }

            var portfolioModel = new Portfolio // Create a new Portfolio object
            {
                StockId = stock.Id, // Set the StockId property to the ID of the stock
                AppUserId = AppUser.Id // Set the AppUserId property to the ID of the user
            };

            await _portfolioRepo.CreateAsync(portfolioModel); // Save the new Portfolio object to the database using the IPortfolioRepository

            if (portfolioModel == null) // Check if the portfolioModel was successfully created (it should never be null in this context, though)
            {
                return StatusCode(500, "Could not create"); // If the creation failed, return a 500 Internal Server Error response with an error message
            }
            else
            {
                // Return a 201 Created response indicating the new resource was successfully created
                // Typically, the Created method would include the URI of the newly created resource and the resource itself
                return Created();
            }
        }
    }
}

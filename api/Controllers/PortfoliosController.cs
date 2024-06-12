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
    [ApiController]
    public class PortfoliosController : ControllerBase
    {
        //Stock repo is not yet used, but there might be a need in the future
        private readonly ICommentRepository _commentRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;
        private readonly IPortfolioRepository _portfolioRepo;
        public PortfoliosController(UserManager<AppUser> userManager,
        IStockRepository stockRepo,
        ICommentRepository commentRepo,
        IPortfolioRepository portfolioRepo)
        {
            _userManager = userManager;
            _stockRepo = stockRepo;
            _commentRepo = commentRepo;
            _portfolioRepo = portfolioRepo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUsername();
            var AppUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(AppUser);
            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.GetUsername();
            var AppUser = await _userManager.FindByNameAsync(username);
            //video 27 exctly 3:00 minutes
            var stock = await _stockRepo.GetBySymbolAsync(symbol);

            if (stock == null)
            {
                return BadRequest("Stock not found");
            }

            var userPortfolio = await _portfolioRepo.GetUserPortfolio(AppUser);

            if (userPortfolio.Any(e => e.Symbol.ToLower() == symbol.ToLower()))
            {
                return BadRequest("The stock already exits in your portfolio");
            }

            var portfolioModel = new Portfolio
            {
                StockId = stock.Id,
                AppUserId = AppUser.Id
            };

            await _portfolioRepo.CreateAsync(portfolioModel);

            if (portfolioModel == null)
            { return StatusCode(500, "Could not create"); }
            else 
            { return Created(); }
        }
    }
}
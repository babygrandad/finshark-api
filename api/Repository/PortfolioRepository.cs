using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDBContext _context;
        public PortfolioRepository(ApplicationDBContext context)
        {
            _context = context;
            
        }

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            await _context.Portfolios.AddAsync(portfolio); // this one adds the "new portfoloModel" that was created in the portfolio controller under the AddPortfolio [HttpPost] to be tracked by entityFrameWork
            await _context.SaveChangesAsync(); // this takes the tracked portfolioModel Object and saves it to the Database
            return portfolio; //this returns the object back to the user so they can use it as confirmation on the json being brought back to the browser.
        }

        public async Task<List<Stock>> GetUserPortfolio(AppUser user)
        {
            return await _context.Portfolios.Where(u => u.AppUserId == user.Id).Select(stock => new Stock
            {
                Id = stock.StockId,
                Symbol = stock.Stock.Symbol,
                CompanyName = stock.Stock.CompanyName,
                Purchase = stock.Stock.Purchase,
                LastDiv = stock.Stock.LastDiv,
                Industry = stock.Stock.Industry,
                MarketCap = stock.Stock.MarketCap
            }).ToListAsync();
        }
    }
}

///you're on video 27 T 7:48

//gone at 11:07
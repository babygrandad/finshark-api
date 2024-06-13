using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.interfaces
{
    public interface IPortfolioRepository
    {
        Task<List<Stock>> GetUserPortfolio(AppUser user);
        Task<Portfolio> CreateAsync(Portfolio portfolio);
    }
}

/*
 - Task :This indicates that the method is asynchronous. In C#, an asynchronous method returns a Task or Task<T>. Using Task allows the method to run asynchronously, meaning it can perform its operations without blocking the calling thread. This is useful for I/O-bound operations such as database calls, which can take some time to complete.
 - <List<Stock>> or <Portfolio> : this is the type of the result that the task will produce/retun when it completes. In this case, the method will return an object{
    - <List<Stock> : it will return a list of stock objects,
    - <Portfolio> : it will return a Portfolio object}
*/
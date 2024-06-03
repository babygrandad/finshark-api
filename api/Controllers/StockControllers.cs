using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockControllers : ControllerBase
    {   
        private readonly ApplicationDBContext _context;
        public StockControllers(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet] //CRUD - Read
        public IActionResult GetAll()
        {
            var stocks =  _context.Stock.ToList();
            return Ok(stocks);
        }

        [HttpGet("{id}")] //CRUD - Read
        public IActionResult GetById([FromRoute] int id)
        {
            var stock =  _context.Stock.Find(id);
            
            if(stock == null)
            {
                return NotFound();
            }

            return Ok(stock);
            

        }
    }
}
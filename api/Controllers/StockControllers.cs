using api.Data; // Importing the data layer
using api.Dtos.Stock; // Importing DTOs for stock operations
using api.Helpers; // Importing helper classes
using api.interfaces; // Importing custom interfaces
using api.Mappers; // Importing mappers for converting models to DTOs
using Microsoft.AspNetCore.Authorization; // Importing authorization attributes
using Microsoft.AspNetCore.Mvc; // Importing MVC framework

namespace api.Controllers
{
    [Route("api/stock")] // Defining the base route for the Stock controller
    [ApiController] // Indicating this is an API controller with automatic model validation
    public class StockControllers : ControllerBase
    {
        private readonly ApplicationDBContext _context; // Field for the database context
        private readonly IStockRepository _stockRepo; // Field for the stock repository

        // Constructor for injecting dependencies
        public StockControllers(ApplicationDBContext context, IStockRepository stockRepo)
        {
            _stockRepo = stockRepo; // Initializing the stock repository
            _context = context; // Initializing the database context
        }

        // GET endpoint to retrieve all stocks, with authorization
        [HttpGet] // CRUD - Read
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query) // Takes query parameters for filtering, sorting, and paging
        {
            if (!ModelState.IsValid) // Check if the model state is valid
            {
                return BadRequest(ModelState); // Return 400 Bad Request if model state is invalid
            }

            var stocks = await _stockRepo.GetAllAsync(query); // Fetch all stocks based on the query object
            var stockDto = stocks.Select(s => s.ToStockDto()); // Convert the stock models to DTOs
            return Ok(stockDto); // Return the stock DTOs with a 200 OK response
        }

        // GET endpoint to retrieve a stock by ID
        [HttpGet("{id:int}")] // CRUD - Read
        public async Task<IActionResult> GetById([FromRoute] int id) // Takes stock ID as a route parameter
        {
            if (!ModelState.IsValid) // Check if the model state is valid
            {
                return BadRequest(ModelState); // Return 400 Bad Request if model state is invalid
            }

            var stock = await _stockRepo.GetByIdAsync(id); // Fetch the stock by ID

            if (stock == null) // Check if the stock exists
            {
                return NotFound(); // Return 404 Not Found if the stock does not exist
            }

            return Ok(stock.ToStockDto()); // Convert the stock model to DTO and return with a 200 OK response
        }

        // POST endpoint to create a new stock
        [HttpPost] // Create
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto) // Takes stock data from the request body
        {
            if (!ModelState.IsValid) // Check if the model state is valid
            {
                return BadRequest(ModelState); // Return 400 Bad Request if model state is invalid
            }

            var stockModel = stockDto.ToStockFromCreateDTO(); // Convert the CreateStockRequestDto to a stock model
            await _stockRepo.CreateAsync(stockModel); // Create the stock in the repository
            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel); // Return 201 Created response with the location header
        }

        // PUT endpoint to update an existing stock
        [HttpPut] // Update
        [Route("{id:int}")] // Takes stock ID as a route parameter
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto) // Takes updated stock data from the request body
        {
            if (!ModelState.IsValid) // Check if the model state is valid
            {
                return BadRequest(ModelState); // Return 400 Bad Request if model state is invalid
            }

            var stockModel = await _stockRepo.UpdateAsync(id, updateDto); // Update the stock in the repository

            if (stockModel == null) // Check if the stock exists and was updated
            {
                return NotFound(); // Return 404 Not Found if the stock does not exist
            }

            return Ok(stockModel.ToStockDto()); // Convert the updated stock model to DTO and return with a 200 OK response
        }

        // DELETE endpoint to delete a stock
        [HttpDelete] // Delete
        [Route("{id:int}")] // Takes stock ID as a route parameter
        public async Task<IActionResult> Delete([FromRoute] int id) // Takes stock ID as a route parameter
        {
            if (!ModelState.IsValid) // Check if the model state is valid
            {
                return BadRequest(ModelState); // Return 400 Bad Request if model state is invalid
            }

            var stockModel = await _stockRepo.DeleteAsync(id); // Delete the stock in the repository

            if (stockModel == null) // Check if the stock exists and was deleted
            {
                return NotFound(); // Return 404 Not Found if the stock does not exist
            }

            return NoContent(); // Return 204 No Content if the stock was successfully deleted
        }
    }
}

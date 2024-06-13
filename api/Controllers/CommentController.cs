using api.Dtos.Comment; // Importing DTOs for comments
using api.interfaces; // Importing custom interfaces
using api.Mappers; // Importing mappers for converting models to DTOs
using Microsoft.AspNetCore.Mvc; // Importing MVC framework

namespace api.Controllers
{
    [Route("api/comment")] // Defining the base route for the Comment controller
    [ApiController] // Indicating this is an API controller with automatic model validation
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo; // Field for the comment repository
        private readonly IStockRepository _stockRepo; // Field for the stock repository

        // Constructor for injecting dependencies
        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo)
        {
            _commentRepo = commentRepo; // Initializing the comment repository
            _stockRepo = stockRepo; // Initializing the stock repository
        }

        // GET endpoint to retrieve all comments
        [HttpGet]
        public async Task<IActionResult> GetAll() // Asynchronous method to get all comments
        {
            if (!ModelState.IsValid) // Check if the model state is valid
            {
                return BadRequest(ModelState); // Return 400 Bad Request if model state is invalid
            }

            var comments = await _commentRepo.GetAllAsync(); // Fetch all comments from the repository
            var commentDto = comments.Select(x => x.ToCommentDto()); // Convert the comment models to DTOs
            return Ok(commentDto); // Return the comment DTOs with a 200 OK response
        }

        // GET endpoint to retrieve a comment by ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id) // Asynchronous method to get a comment by ID
        {
            if (!ModelState.IsValid) // Check if the model state is valid
            {
                return BadRequest(ModelState); // Return 400 Bad Request if model state is invalid
            }

            var comment = await _commentRepo.GetByIdAsync(id); // Fetch the comment by ID from the repository

            if (comment == null) // Check if the comment exists
            {
                return NotFound(); // Return 404 Not Found if the comment does not exist
            }

            return Ok(comment.ToCommentDto()); // Convert the comment model to DTO and return with a 200 OK response
        }

        // POST endpoint to create a new comment for a stock
        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentDto commentDto) // Takes stock ID as a route parameter and comment data from the request body
        {
            if (!ModelState.IsValid) // Check if the model state is valid
            {
                return BadRequest(ModelState); // Return 400 Bad Request if model state is invalid
            }

            if (!await _stockRepo.StockExit(stockId)) // Check if the stock exists in the repository
            {
                return BadRequest("Stock does not exist!"); // Return 400 Bad Request if the stock does not exist
            }

            var commentModel = commentDto.ToCommentFromCreate(stockId); // Convert the CreateCommentDto to a comment model
            await _commentRepo.CreateAsync(commentModel); // Create the comment in the repository
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto()); // Return 201 Created response with the location header
        }

        // PUT endpoint to update an existing comment
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto) // Takes comment ID as a route parameter and updated comment data from the request body
        {
            if (!ModelState.IsValid) // Check if the model state is valid
            {
                return BadRequest(ModelState); // Return 400 Bad Request if model state is invalid
            }

            var comment = await _commentRepo.UpdateAsync(id, updateDto.ToCommentFromUpdate()); // Update the comment in the repository

            if (comment == null) // Check if the comment exists and was updated
            {
                return NotFound("Comment not found."); // Return 404 Not Found if the comment does not exist
            }

            return Ok(comment.ToCommentDto()); // Convert the updated comment model to DTO and return with a 200 OK response
        }

        // DELETE endpoint to delete a comment
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id) // Takes comment ID as a route parameter
        {
            if (!ModelState.IsValid) // Check if the model state is valid
            {
                return BadRequest(ModelState); // Return 400 Bad Request if model state is invalid
            }

            var commentModel = await _commentRepo.DeleteAsync(id); // Delete the comment in the repository

            if (commentModel == null) // Check if the comment exists and was deleted
            {
                return NotFound("Comment not found."); // Return 404 Not Found if the comment does not exist
            }

            return Ok(commentModel); // Return the deleted comment model with a 200 OK response
        }
    }
}

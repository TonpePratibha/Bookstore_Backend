
using BuisnessLayer.Interface;
using BuisnessLayer.Service;
using DataAccessLayer.CustomException;
using DataAccessLayer.Entity;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BookController : ControllerBase

    {

        private readonly IBookService _bookService;
        private readonly ILogger<BookController> _logger;


        public BookController(IBookService bookService, ILogger<BookController> logger)
        {
            _bookService = bookService;
            _logger = logger;

        }


        [HttpPost("loadfromcsv")]
        [Authorize]
        public IActionResult LoadBooksFromCsv()
        {
            _logger.LogInformation("Loading books from CSV.");
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = _bookService.LoadBooksFromCsv(token);
            if (result.StartsWith("Unauthorized"))
                return Unauthorized(result);

            _logger.LogInformation("Books loaded from CSV successfully.");
            return Ok(result);
        }

      

        [HttpPost]
        [Authorize]
        public IActionResult AddBook([FromBody] BookModel model)
        {
            try
            {
                _logger.LogInformation("Attempting to add a new book.");
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    _logger.LogWarning("Token not found or invalid.");
                    return Unauthorized(new { status = false, message = "Token not found or user is not logged in" });
                }

                string token = authHeader.Replace("Bearer ", "");

                var result = _bookService.AddBook(model, token);
                _logger.LogInformation("Book added successfully.");
                return Ok(new { status = true, message = "Book added", data = result });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("UnauthorizedAccessException: User is unauthorized.");
                return StatusCode(403, new { status = false, message = "User is unauthorized" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding book.");
                return StatusCode(500, new { status = false, message = ex.Message });
            }
        }


        [HttpPut("{id}")]
        
        public IActionResult UpdateBook(int id, [FromBody] BookModel model)
        {
            try
            {

                _logger.LogInformation($"Updating book with ID: {id}");
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    _logger.LogWarning("Token not found or invalid.");
                    return Unauthorized(new { status = false, message = "Token not found or user is not logged in" });
                }

                string token = authHeader.Replace("Bearer ", "");
                var result = _bookService.UpdateBook(id, model, token);
                _logger.LogInformation("Book updated successfully.");
                return Ok(new { status = true, message = "Book updated", data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt.");
                return StatusCode(403, new { status = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating book.");
                return StatusCode(500, new { status = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        
        public IActionResult DeleteBook(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting book with ID: {id}");
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    _logger.LogWarning("Token not found or invalid.");
                    return Unauthorized(new { status = false, message = "Token not found or user is not logged in" });
                }

                string token = authHeader.Replace("Bearer ", "");
               // string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = _bookService.DeleteBook(id, token);
                _logger.LogInformation("Book deleted successfully.");
                return Ok(new { status = true, message = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt.");
                return StatusCode(403, new { status = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting book.");
                return StatusCode(500, new { status = false, message = ex.Message });
            }
        }



        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                _logger.LogInformation("Fetching all books.");
                var books = _bookService.GetAllBooks();
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books.");
                return StatusCode(500, $"Error retrieving books: {ex.Message}");
            }
        }
       
        [HttpGet("pages")]
        public IActionResult GetAllBooksWithPage(int page)
        {
            try
            {
                _logger.LogInformation($"Fetching books on page: {page}");
                var result = _bookService.GetAllBooksWithPage(page);
                return Ok(result);
            }
            catch (PageNotFoundException ex)
            {
                _logger.LogWarning(ex, "Page not found.");
                return NotFound(new { status = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books by page.");
                return StatusCode(500, new { status = false, message = $"Error retrieving books: {ex.Message}" });
            }
        }


        [HttpGet("recent")]
        public IActionResult GetMostRecentBook()
        {
            try
            {
                _logger.LogInformation("Fetching most recent books.");
                var books = _bookService.GetAllRecentBooks();

                if (books == null)

                    return NotFound("No books found.");

                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving most recent books.");
                return StatusCode(500, "An error occurred while retrieving the most recent book: " + ex.Message);
            }
        }


        
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                _logger.LogInformation($"Fetching book with ID: {id}");
                var book = _bookService.GetBookById(id);
                return Ok(book);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book by ID.");
                return StatusCode(500, new { message = "An error occurred while retrieving the book.", error = ex.Message });
            }
        }



        [HttpGet("search")]
        public IActionResult SearchBooks([FromQuery] string search)
        {
            try
            {
                _logger.LogInformation($"Searching for books with keyword: {search}");
                var books = _bookService.SearchBooks(search);
                if (!books.Any())
                {
                    return NotFound("No books found for this author or title");
                }
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during book search.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet("sortprice_asc")]
        public IActionResult GetBooksSortedByPriceAsc()
        {
            try
            {
                _logger.LogInformation("Fetching books sorted by price (ascending).");
                var books = _bookService.GetBooksSortedByPriceAsc();
                if (!books.Any())
                {
                    return NotFound("No books found.");
                }
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books by ascending price.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("sortprice_desc")]
        public IActionResult GetBooksSortedByPriceDesc()
        {
            try
            {
                _logger.LogInformation("Fetching books sorted by price (descending).");
                var books = _bookService.GetBooksSortedByPriceDesc();
                if (!books.Any())
                {
                    return NotFound("No books found.");
                }
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books by descending price.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}

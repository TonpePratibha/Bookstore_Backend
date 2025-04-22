using BuisnessLayer.Interface;
using BuisnessLayer.Service;
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


        public BookController(IBookService bookService)
        {
            _bookService = bookService;


        }


        [HttpPost("loadfromcsv")]
        [Authorize]
        public IActionResult LoadBooksFromCsv()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = _bookService.LoadBooksFromCsv(token);
            if (result.StartsWith("Unauthorized"))
                return Unauthorized(result);

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddBook([FromBody] BookModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = _bookService.AddBook(model, token);
                return Ok(new { status = true, message = "Book added", data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { status = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateBook(int id, [FromBody] BookModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = _bookService.UpdateBook(id, model, token);
                return Ok(new { status = true, message = "Book updated", data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { status = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteBook(int id)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = _bookService.DeleteBook(id, token);
                return Ok(new { status = true, message = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { status = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = ex.Message });
            }
        }



        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _bookService.GetAllBooks();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving books: {ex.Message}");
            }
        }

        [HttpGet("pages")]
        public IActionResult GetAllBooksWithPage(int page = 1, int pageSize = 6)
        {
            try
            {
                var books = _bookService.GetAllBooksWithPage(page, pageSize);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = $"Error retrieving books: {ex.Message}" });
            }
        }





        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var book = _bookService.GetBookById(id);
                return Ok(book);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the book.", error = ex.Message });
            }
        }



        [HttpGet("searchbyauthor")]
        public IActionResult SearchBooksByAuthor([FromQuery] string author)
        {
            try
            {
                var books = _bookService.SearchBooksByAuthor(author);
                if (!books.Any())
                {
                    return NotFound("No books found for this author.");
                }
                return Ok(books);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpGet("searchbytitle")]
        public IActionResult SearchBooksByTitle([FromQuery] string title)
        {
            try
            {
                var books = _bookService.SearchBooksByTitle(title);
                if (!books.Any())
                {
                    return NotFound("No books found with this title.");
                }
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("sortprice_asc")]
        public IActionResult GetBooksSortedByPriceAsc()
        {
            try
            {
                var books = _bookService.GetBooksSortedByPriceAsc();
                if (!books.Any())
                {
                    return NotFound("No books found.");
                }
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("sortprice_desc")]
        public IActionResult GetBooksSortedByPriceDesc()
        {
            try
            {
                var books = _bookService.GetBooksSortedByPriceDesc();
                if (!books.Any())
                {
                    return NotFound("No books found.");
                }
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}
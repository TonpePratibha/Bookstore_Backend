using BuisnessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public IActionResult UploadBooksFromCsv()
        {
            try
            {
                var result = _bookService.LoadBooksFromCsv();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Exception: {ex.Message}");
            }
        }

    }
}

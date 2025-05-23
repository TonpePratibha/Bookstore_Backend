﻿using BuisnessLayer.Interface;
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

        /*  [HttpPost]
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
                  return StatusCode(403, new { status = false, message = "token not found user is not unauthorized" });
              }
              catch (Exception ex)
              {
                  return StatusCode(500, new { status = false, message = ex.Message });
              }
          }
        */

        [HttpPost]
        [Authorize]
        public IActionResult AddBook([FromBody] BookModel model)
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { status = false, message = "Token not found or user is not logged in" });
                }

                string token = authHeader.Replace("Bearer ", "");

                var result = _bookService.AddBook(model, token);
                return Ok(new { status = true, message = "Book added", data = result });
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new { status = false, message = "User is unauthorized" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = ex.Message });
            }
        }


        [HttpPut("{id}")]
        
        public IActionResult UpdateBook(int id, [FromBody] BookModel model)
        {
            try
            {


                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { status = false, message = "Token not found or user is not logged in" });
                }

                string token = authHeader.Replace("Bearer ", "");
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
        
        public IActionResult DeleteBook(int id)
        {
            try
            {

                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { status = false, message = "Token not found or user is not logged in" });
                }

                string token = authHeader.Replace("Bearer ", "");
               // string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
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
        /*
                [HttpGet("pages")]
                public IActionResult GetAllBooksWithPage(int page)
                {
                    try
                    {
                        var books = _bookService.GetAllBooksWithPage(page);
                        return Ok(books);
                    }
                    catch (PageNotFoundException ex)
                    {
                        return NotFound(new { status = false, message = ex.Message });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { status = false, message = $"Error retrieving books: {ex.Message}" });
                    }
                }
                */
        [HttpGet("pages")]
        public IActionResult GetAllBooksWithPage(int page)
        {
            try
            {
                var result = _bookService.GetAllBooksWithPage(page);
                return Ok(result);
            }
            catch (PageNotFoundException ex)
            {
                return NotFound(new { status = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = $"Error retrieving books: {ex.Message}" });
            }
        }


        [HttpGet("recent")]
        public IActionResult GetMostRecentBook()
        {
            try
            {
                var books = _bookService.GetAllRecentBooks();

                if (books == null)
                    return NotFound("No books found.");

                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the most recent book: " + ex.Message);
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



        [HttpGet("search")]
        public IActionResult SearchBooks([FromQuery] string search)
        {
            try
            {
                var books = _bookService.SearchBooks(search);
                if (!books.Any())
                {
                    return NotFound("No books found for this author or title");
                }
                return Ok(books);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


/*
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
*/

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
using bookapi.Context;
using bookapi.DTO;
using bookapi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bookapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var books = await context.Books.Include(b => b.Author).ToListAsync();

            if (books == null)
            {
                return BadRequest("No books found");
            }

            var result = books.Select(b => 
            new {
                Title = b.Title,
                PublishedDate = b.PublishedDate,
                AuthorName = b.Author?.Name
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var book = context.Books.Include(b => b.Author).SingleOrDefault(x => x.Id == id);

            if (book == null)
            {
                return BadRequest("No book found");
            }

            var result = new
            {
                Title = book.Title,
                PublishedDate = book.PublishedDate,
                AuthorName = book.Author?.Name
            };

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] BookDTO bookDto)
        {
            if (bookDto == null) return BadRequest("Input empty");
            var book = new Book
            {
                Title = bookDto.Title,
                PublishedDate = bookDto.PublishedDate,
                AuthorId = bookDto.AuthorId,
            };

            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] BookDTO bookDto)
        {
            var bookToUpdate = context.Books.SingleOrDefault(x => x.Id == id);
            if (bookToUpdate == null) return BadRequest("No book found");

            bookToUpdate.AuthorId = bookDto.AuthorId;
            bookToUpdate.Title = bookDto.Title;
            bookToUpdate.PublishedDate = bookDto.PublishedDate;

            await context.SaveChangesAsync();
            return Ok("Update successfull");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var bookToDelete = context.Books.SingleOrDefault(x => x.Id == id);
            if (bookToDelete == null) return BadRequest("No book found");

            context.Books.Remove(bookToDelete);
            await context.SaveChangesAsync();

            return Ok("Removed Successfully");
        }
    }
}
using bookapi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bookapi.Context;
using bookapi.DTO;
using System.Security.Claims;

namespace bookapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var authors = await context.Authors.Include(a => a.Books).ToListAsync();

            if (authors == null)
            {
                return BadRequest("No authors found");
            }

            var result = authors.Select(a => 
            new {
                Name = a.Name,
                BirthDate = a.BirthDate,
                BookTitles = a.Books.Select(b => b.Title).ToList()
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var author = context.Authors.Include(a => a.Books).SingleOrDefault(x => x.Id == id);

            if (author == null)
            {
                return BadRequest("No author found");
            }

            var result = new
            {
                Name = author.Name,
                BirthDate = author.BirthDate,
                BookTitles = author.Books.Select(b => b.Title).ToList()
            };

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] AuthorDTO authorDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (authorDto == null) return BadRequest("Input empty");
            var author = new Author
            {
                Name = authorDto.Name,
                BirthDate = authorDto.BirthDate,
                UserId = userId!
            };

            await context.Authors.AddAsync(author);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] AuthorDTO authorDto)
        {
            var authorToUpdate = context.Authors.SingleOrDefault(x => x.Id == id);
            if (authorToUpdate == null) return BadRequest("No author found");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            authorToUpdate.Name = authorDto.Name;
            authorToUpdate.BirthDate = authorDto.BirthDate;
            authorToUpdate.UserId = userId!;

            await context.SaveChangesAsync();
            return Ok("Update successfull");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var authorToDelete = context.Authors.SingleOrDefault(x => x.Id == id);
            if (authorToDelete == null) return BadRequest("No author found");

            context.Authors.Remove(authorToDelete);
            await context.SaveChangesAsync();

            return Ok("Removed Successfully");
        }
    }
}
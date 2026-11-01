using Library.Data.Context;
using Library.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApi.DTOs;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly LibraryDbContext _db;

        public AuthorsController(LibraryDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor(CreateAuthorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                dto.Name.Length > 40 ||
                string.IsNullOrWhiteSpace(dto.Surname) ||
                dto.Surname.Length > 60)
                return BadRequest("Invalid author data.");

            var author = new Tauthor
            {
                CName = dto.Name.Trim(),
                CSurname = dto.Surname.Trim()
            };

            _db.Tauthors.Add(author);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAuthorById),
                new { id = author.NAuthorId },
                Resource(author));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await _db.Tauthors
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.NAuthorId == id);

            return author == null
                ? NotFound()
                : Ok(Resource(author));
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {
            var authors = await _db.Tauthors
                .AsNoTracking()
                .OrderBy(a => a.NAuthorId)
                .ToListAsync();

            return Ok(new
            {
                Items = authors.Select(Resource),
                _links = new Dictionary<string, HateoasLink>
                {
                    ["self"] = HateoasHelper.Link(
                        "/api/authors", "GET", "List authors"),

                    ["create"] = HateoasHelper.Link(
                        "/api/authors", "POST", "Create an author")
                }
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAuthor(
            int id, UpdateAuthorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                dto.Name.Length > 40 ||
                string.IsNullOrWhiteSpace(dto.Surname) ||
                dto.Surname.Length > 60)
                return BadRequest("Invalid author data.");

            var author = await _db.Tauthors.FindAsync(id);

            if (author == null)
                return NotFound();

            author.CName = dto.Name.Trim();
            author.CSurname = dto.Surname.Trim();

            await _db.SaveChangesAsync();

            return Ok(Resource(author));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _db.Tauthors.FindAsync(id);

            if (author == null)
                return NotFound();

            if (await _db.Tbooks.AnyAsync(b => b.NAuthorId == id))
                return Conflict("Cannot delete an author who has books.");

            _db.Tauthors.Remove(author);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                Message = "Author deleted successfully.",
                _links = new Dictionary<string, HateoasLink>
                {
                    ["all-authors"] = HateoasHelper.Link(
                        "/api/authors", "GET", "List authors"),

                    ["create"] = HateoasHelper.Link(
                        "/api/authors", "POST", "Create an author")
                }
            });
        }

        private static HateoasResource<AuthorDto> Resource(
            Tauthor author)
        {
            var dto = new AuthorDto(
                author.NAuthorId,
                author.CName,
                author.CSurname);

            var links = new Dictionary<string, HateoasLink>
            {
                ["self"] = HateoasHelper.Link(
                    $"/api/authors/{author.NAuthorId}",
                    "GET", "Read author"),

                ["update"] = HateoasHelper.Link(
                    $"/api/authors/{author.NAuthorId}",
                    "PUT", "Update author"),

                ["delete"] = HateoasHelper.Link(
                    $"/api/authors/{author.NAuthorId}",
                    "DELETE", "Delete author"),

                ["all-authors"] = HateoasHelper.Link(
                    "/api/authors", "GET", "List authors")
            };

            return HateoasHelper.Resource(dto, links);
        }
    }
}

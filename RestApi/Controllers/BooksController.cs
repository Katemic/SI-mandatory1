using Library.Data.Context;
using Library.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApi.DTOs;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _db;

        public BooksController(LibraryDbContext db)
        {
            _db = db;
        }

        // POST: api/books
        [HttpPost]
        public async Task<IActionResult> CreateBook(CreateBookDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) ||
                dto.Title.Length > 255 ||
                dto.AuthorId <= 0 ||
                dto.PublisherId <= 0 ||
                dto.PublishingYear <= 0 ||
                dto.PublishingYear > 9999)
            {
                return BadRequest("Invalid book data.");
            }

            if (!await _db.Tauthors.AnyAsync(
                a => a.NAuthorId == dto.AuthorId))
                return BadRequest("Author does not exist.");

            if (!await _db.Tpublishingcompanies.AnyAsync(
                p => p.NPublishingCompanyId == dto.PublisherId))
                return BadRequest("Publisher does not exist.");

            var book = new Tbook
            {
                CTitle = dto.Title.Trim(),
                NAuthorId = dto.AuthorId,
                NPublishingCompanyId = dto.PublisherId,
                NPublishingYear = dto.PublishingYear
            };

            _db.Tbooks.Add(book);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetBookById),
                new { id = book.NBookId },
                Resource(ToDto(book)));
        }

        // GET: api/books/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _db.Tbooks
                .AsNoTracking()
                .Where(b => b.NBookId == id)
                .Select(b => new BookDto(
                    b.NBookId,
                    b.CTitle,
                    b.NAuthorId,
                    b.NPublishingCompanyId,
                    b.NPublishingYear))
                .FirstOrDefaultAsync();

            if (book == null)
                return NotFound();

            return Ok(Resource(book));
        }

        // GET: api/books?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetBooks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest("Invalid page or pageSize.");

            // Total number of books
            var totalCount = await _db.Tbooks.CountAsync();

            // Calculate total pages
            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize);

            // Get books for current page
            var books = await _db.Tbooks
                .AsNoTracking()
                .OrderBy(b => b.NBookId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookDto(
                    b.NBookId,
                    b.CTitle,
                    b.NAuthorId,
                    b.NPublishingCompanyId,
                    b.NPublishingYear))
                .ToListAsync();

            // Add HATEOAS links to every book
            var items = books.Select(book => Resource(book)).ToList();

            // Top-level HATEOAS links
            var links = new Dictionary<string, HateoasLink>
            {
                ["self"] = HateoasHelper.Link(
                    $"/api/books?page={page}&pageSize={pageSize}",
                    "GET",
                    "Current page"),

                ["create"] = HateoasHelper.Link(
                    "/api/books",
                    "POST",
                    "Create a book")
            };

            if (page > 1)
            {
                links["previous"] = HateoasHelper.Link(
                    $"/api/books?page={page - 1}&pageSize={pageSize}",
                    "GET",
                    "Previous page");
            }

            if (page < totalPages)
            {
                links["next"] = HateoasHelper.Link(
                    $"/api/books?page={page + 1}&pageSize={pageSize}",
                    "GET",
                    "Next page");
            }

            if (totalPages > 0)
            {
                links["first"] = HateoasHelper.Link(
                    $"/api/books?page=1&pageSize={pageSize}",
                    "GET",
                    "First page");

                links["last"] = HateoasHelper.Link(
                    $"/api/books?page={totalPages}&pageSize={pageSize}",
                    "GET",
                    "Last page");
            }

            var response = new PagedResponseDto<BookDto>
            {
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPreviousPage = page > 1,
                HasNextPage = page < totalPages,
                Items = items,
                Links = links
            };

            return Ok(response);
        }

        // PUT: api/books/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBook(
            int id, UpdateBookDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) ||
                dto.Title.Length > 255 ||
                dto.AuthorId <= 0 ||
                dto.PublisherId <= 0 ||
                dto.PublishingYear <= 0 ||
                dto.PublishingYear > 9999)
            {
                return BadRequest("Invalid book data.");
            }

            var book = await _db.Tbooks.FindAsync(id);

            if (book == null)
                return NotFound();

            if (!await _db.Tauthors.AnyAsync(
                a => a.NAuthorId == dto.AuthorId))
                return BadRequest("Author does not exist.");

            if (!await _db.Tpublishingcompanies.AnyAsync(
                p => p.NPublishingCompanyId == dto.PublisherId))
                return BadRequest("Publisher does not exist.");

            book.CTitle = dto.Title.Trim();
            book.NAuthorId = dto.AuthorId;
            book.NPublishingCompanyId = dto.PublisherId;
            book.NPublishingYear = dto.PublishingYear;

            await _db.SaveChangesAsync();

            return Ok(Resource(ToDto(book)));
        }

        // DELETE: api/books/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _db.Tbooks.FindAsync(id);

            if (book == null)
                return NotFound();

            _db.Tbooks.Remove(book);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                Message = "Book deleted successfully.",
                _links = new Dictionary<string, HateoasLink>
                {
                    ["all-books"] = HateoasHelper.Link(
                        "/api/books?page=1&pageSize=10",
                        "GET",
                        "List books"),

                    ["create"] = HateoasHelper.Link(
                        "/api/books",
                        "POST",
                        "Create a book")
                }
            });
        }

        // Convert database model to DTO
        private static BookDto ToDto(Tbook book)
        {
            return new BookDto(
                book.NBookId,
                book.CTitle,
                book.NAuthorId,
                book.NPublishingCompanyId,
                book.NPublishingYear);
        }

        // Individual book HATEOAS resource
        private static HateoasResource<BookDto> Resource(BookDto book)
        {
            var links = new Dictionary<string, HateoasLink>
            {
                ["self"] = HateoasHelper.Link(
                    $"/api/books/{book.Id}", "GET", "Read this book"),

                ["update"] = HateoasHelper.Link(
                    $"/api/books/{book.Id}", "PUT", "Update this book"),

                ["delete"] = HateoasHelper.Link(
                    $"/api/books/{book.Id}", "DELETE", "Delete this book"),

                ["author"] = HateoasHelper.Link(
                    $"/api/authors/{book.AuthorId}", "GET", "Read author"),

                ["publisher"] = HateoasHelper.Link(
                    $"/api/publishers/{book.PublisherId}", "GET", "Read publisher"),

                ["all-books"] = HateoasHelper.Link(
                    "/api/books?page=1&pageSize=10", "GET", "List books")
            };

            return HateoasHelper.Resource(book, links);
        }
    }
}

using Library.Data.Context;
using Library.Data.Models;
using Microsoft.EntityFrameworkCore;
using RestApi.DTOs;
using static System.Reflection.Metadata.BlobBuilder;

namespace RestApi.Endpoints
{
    public static class BookEndpoints
    {
        public static void MapBookEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/books").WithTags("Books");

            // POST /api/books
            group.MapPost("/", async (CreateBookDto dto, LibraryDbContext db) =>
            {
                if (string.IsNullOrWhiteSpace(dto.Title) ||
                    dto.Title.Length > 255 ||
                    dto.AuthorId <= 0 ||
                    dto.PublisherId <= 0 ||
                    dto.PublishingYear <= 0 ||
                    dto.PublishingYear > 9999)
                    return Results.BadRequest("Invalid book data.");

                if (!await db.Tauthors.AnyAsync(a => a.NAuthorId == dto.AuthorId))
                    return Results.BadRequest("Author does not exist.");

                if (!await db.Tpublishingcompanies.AnyAsync(
                    p => p.NPublishingCompanyId == dto.PublisherId))
                    return Results.BadRequest("Publisher does not exist.");

                var book = new Tbook
                {
                    CTitle = dto.Title.Trim(),
                    NAuthorId = dto.AuthorId,
                    NPublishingCompanyId = dto.PublisherId,
                    NPublishingYear = dto.PublishingYear
                };

                db.Tbooks.Add(book);
                await db.SaveChangesAsync();

                var result = ToDto(book);

                var response = HateoasHelper.Resource(result, new()
                {
                    ["self"] = HateoasHelper.Link($"/api/books/{book.NBookId}", "GET", "Read this book"),
                    ["update"] = HateoasHelper.Link($"/api/books/{book.NBookId}", "PUT", "Update this book"),
                    ["delete"] = HateoasHelper.Link($"/api/books/{book.NBookId}", "DELETE", "Delete this book"),
                    ["all-books"] = HateoasHelper.Link("/api/books?limit=10&offset=0", "GET", "List books")
                });

                return Results.Created($"/api/books/{book.NBookId}", response);
            }).WithName("CreateBook");

            // GET /api/books/{id}
            group.MapGet("/{id:int}", async (int id, LibraryDbContext db) =>
            {
                var book = await db.Tbooks.AsNoTracking()
                    .Where(b => b.NBookId == id)
                    .Select(b => new BookDto(
                        b.NBookId,
                        b.CTitle,
                        b.NAuthorId,
                        b.NPublishingCompanyId,
                        b.NPublishingYear))
                    .FirstOrDefaultAsync();

                if (book is null)
                    return Results.NotFound();

                var response = HateoasHelper.Resource(book, new()
                {
                    ["self"] = HateoasHelper.Link($"/api/books/{id}", "GET", "Read this book"),
                    ["update"] = HateoasHelper.Link($"/api/books/{id}", "PUT", "Update this book"),
                    ["delete"] = HateoasHelper.Link($"/api/books/{id}", "DELETE", "Delete this book"),
                    ["all-books"] = HateoasHelper.Link("/api/books?limit=10&offset=0", "GET", "List books"),
                    ["author"] = HateoasHelper.Link($"/api/authors/{book.AuthorId}", "GET", "Read author"),
                    ["publisher"] = HateoasHelper.Link($"/api/publishers/{book.PublisherId}", "GET", "Read publisher")
                });

                return Results.Ok(response);
            }).WithName("GetBookById");

            // GET /api/books?limit=10&offset=0
            group.MapGet("/", async (
                LibraryDbContext db, int limit = 10, int offset = 0) =>
            {
                if (limit < 1 || limit > 100 || offset < 0)
                    return Results.BadRequest("Invalid limit or offset.");

                var total = await db.Tbooks.CountAsync();

                var books = await db.Tbooks.AsNoTracking().OrderBy(b => b.NBookId).Skip(offset).Take(limit)
                    .Select(b => new BookDto(
                        b.NBookId,
                        b.CTitle,
                        b.NAuthorId,
                        b.NPublishingCompanyId,
                        b.NPublishingYear)).ToListAsync();

                var items = books.Select(book =>
                    HateoasHelper.Resource(book, new()
                    {
                        ["self"] = HateoasHelper.Link($"/api/books/{book.Id}", "GET", "Read this book"),
                        ["update"] = HateoasHelper.Link($"/api/books/{book.Id}", "PUT", "Update this book"),
                        ["delete"] = HateoasHelper.Link($"/api/books/{book.Id}", "DELETE", "Delete this book"),
                        ["author"] = HateoasHelper.Link($"/api/authors/{book.AuthorId}", "GET", "Read author"),
                        ["publisher"] = HateoasHelper.Link($"/api/publishers/{book.PublisherId}", "GET", "Read publisher")
                    })).ToList();

                var links = new Dictionary<string, HateoasLink>
                {
                    ["self"] = HateoasHelper.Link($"/api/books?limit={limit}&offset={offset}", "GET", "Current page"),
                    ["create"] = HateoasHelper.Link("/api/books", "POST", "Create a book")
                };

                if (offset > 0)
                    links["previous"] = HateoasHelper.Link($"/api/books?limit={limit}&offset={Math.Max(0, offset - limit)}", "GET", "Previous page");

                if (offset + limit < total)
                    links["next"] = HateoasHelper.Link($"/api/books?limit={limit}&offset={offset + limit}", "GET", "Next page");

                return Results.Ok(new HateoasCollection<BookDto>
                {
                    TotalCount = total,
                    Limit = limit,
                    Offset = offset,
                    Items = items,
                    Links = links
                });
            }).WithName("GetBooks");

            // PUT /api/books/{id}
            group.MapPut("/{id:int}", async (
                int id, UpdateBookDto dto, LibraryDbContext db) =>
            {
                if (string.IsNullOrWhiteSpace(dto.Title) ||
                    dto.Title.Length > 255 ||
                    dto.AuthorId <= 0 ||
                    dto.PublisherId <= 0 ||
                    dto.PublishingYear <= 0 ||
                    dto.PublishingYear > 9999)
                    return Results.BadRequest("Invalid book data.");

                var book = await db.Tbooks.FindAsync(id);
                if (book is null)
                    return Results.NotFound();

                if (!await db.Tauthors.AnyAsync(a => a.NAuthorId == dto.AuthorId))
                    return Results.BadRequest("Author does not exist.");

                if (!await db.Tpublishingcompanies.AnyAsync(p => p.NPublishingCompanyId == dto.PublisherId))
                    return Results.BadRequest("Publisher does not exist.");

                book.CTitle = dto.Title.Trim();
                book.NAuthorId = dto.AuthorId;
                book.NPublishingCompanyId = dto.PublisherId;
                book.NPublishingYear = dto.PublishingYear;

                await db.SaveChangesAsync();

                var response = HateoasHelper.Resource(ToDto(book), new()
                {
                    ["self"] = HateoasHelper.Link($"/api/books/{id}", "GET", "Read this book"),
                    ["update"] = HateoasHelper.Link($"/api/books/{id}", "PUT", "Update this book"),
                    ["delete"] = HateoasHelper.Link($"/api/books/{id}", "DELETE", "Delete this book"),
                    ["all-books"] = HateoasHelper.Link("/api/books?limit=10&offset=0", "GET", "List books")
                });

                return Results.Ok(response);
            }).WithName("UpdateBook");

            // DELETE /api/books/{id}
            group.MapDelete("/{id:int}", async (int id, LibraryDbContext db) =>
            {
                var book = await db.Tbooks.FindAsync(id);
                if (book is null)
                    return Results.NotFound();

                db.Tbooks.Remove(book);
                await db.SaveChangesAsync();

                return Results.Ok(new
                {
                    Message = "Book deleted successfully.",
                    _links = new Dictionary<string, HateoasLink>
                    {
                        ["all-books"] = HateoasHelper.Link("/api/books?limit=10&offset=0", "GET", "List books"),
                        ["create"] = HateoasHelper.Link("/api/books", "POST", "Create a book")
                    }
                });
            }).WithName("DeleteBook");
        }

        private static BookDto ToDto(Tbook book) => new(book.NBookId, book.CTitle,
            book.NAuthorId, book.NPublishingCompanyId, book.NPublishingYear);
    }
}

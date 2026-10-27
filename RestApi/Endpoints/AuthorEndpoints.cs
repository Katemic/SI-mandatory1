using Library.Data.Context;
using Library.Data.Models;
using Microsoft.EntityFrameworkCore;
using RestApi.DTOs;

namespace RestApi.Endpoints
{
    public static class AuthorEndpoints
    {
        public static void MapAuthorEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/authors").WithTags("Authors");

            // POST /api/authors
            group.MapPost("/", async (CreateAuthorDto dto, LibraryDbContext db) =>
            {
                if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > 40 ||
                    string.IsNullOrWhiteSpace(dto.Surname) || dto.Surname.Length > 60)
                    return Results.BadRequest("Invalid author data.");

                var author = new Tauthor
                {
                    CName = dto.Name.Trim(),
                    CSurname = dto.Surname.Trim()
                };

                db.Tauthors.Add(author);
                await db.SaveChangesAsync();

                return Results.Created($"/api/authors/{author.NAuthorId}",Resource(author));
            }).WithName("CreateAuthor");

            // GET /api/authors/{id}
            group.MapGet("/{id:int}", async (int id, LibraryDbContext db) =>
            {
                var author = await db.Tauthors.AsNoTracking().FirstOrDefaultAsync(a => a.NAuthorId == id);

                return author is null
                    ? Results.NotFound()
                    : Results.Ok(Resource(author));
            }).WithName("GetAuthorById");

            // GET /api/authors
            group.MapGet("/", async (LibraryDbContext db) =>
            {
                var authors = await db.Tauthors.AsNoTracking().OrderBy(a => a.NAuthorId).ToListAsync();

                return Results.Ok(new
                {
                    Items = authors.Select(Resource),
                    _links = new Dictionary<string, HateoasLink>
                    {
                        ["self"] = HateoasHelper.Link("/api/authors", "GET", "List authors"),
                        ["create"] = HateoasHelper.Link("/api/authors", "POST", "Create an author")
                    }
                });
            }).WithName("GetAuthors");

            // PUT /api/authors/{id}
            group.MapPut("/{id:int}", async (
                int id, UpdateAuthorDto dto, LibraryDbContext db) =>
            {
                if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > 40 ||
                    string.IsNullOrWhiteSpace(dto.Surname) || dto.Surname.Length > 60)
                    return Results.BadRequest("Invalid author data.");

                var author = await db.Tauthors.FindAsync(id);
                if (author is null)
                    return Results.NotFound();

                author.CName = dto.Name.Trim();
                author.CSurname = dto.Surname.Trim();

                await db.SaveChangesAsync();

                return Results.Ok(Resource(author));
            }).WithName("UpdateAuthor");
            
            // DELETE /api/authors/{id}
            group.MapDelete("/{id:int}", async (int id, LibraryDbContext db) =>
            {
                var author = await db.Tauthors.FindAsync(id);
                if (author is null)
                    return Results.NotFound();

                if (await db.Tbooks.AnyAsync(b => b.NAuthorId == id))
                    return Results.Conflict("Cannot delete an author who has books.");

                db.Tauthors.Remove(author);
                await db.SaveChangesAsync();

                return Results.Ok(new
                {
                    Message = "Author deleted successfully.",
                    _links = new Dictionary<string, HateoasLink>
                    {
                        ["all-authors"] = HateoasHelper.Link("/api/authors", "GET", "List authors"),
                        ["create"] = HateoasHelper.Link("/api/authors", "POST", "Create an author")
                    }
                });
            }).WithName("DeleteAuthor");
        }
        private static HateoasResource<AuthorDto> Resource(Tauthor author)
        {
            var dto = new AuthorDto(
                author.NAuthorId, author.CName, author.CSurname);

            var links = new Dictionary<string, HateoasLink>
            {
                ["self"] = HateoasHelper.Link($"/api/authors/{author.NAuthorId}", "GET", "Read author"),
                ["update"] = HateoasHelper.Link($"/api/authors/{author.NAuthorId}", "PUT", "Update author"),
                ["delete"] = HateoasHelper.Link($"/api/authors/{author.NAuthorId}", "DELETE", "Delete author"),
                ["all-authors"] = HateoasHelper.Link("/api/authors", "GET", "List authors")
            };

            return HateoasHelper.Resource(dto, links);
        }
    }
}

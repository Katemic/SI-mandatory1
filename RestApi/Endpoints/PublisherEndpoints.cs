using Library.Data.Context;
using Library.Data.Models;
using Microsoft.EntityFrameworkCore;
using RestApi.DTOs;

namespace RestApi.Endpoints
{
    public static class PublisherEndpoints
    {
        public static void MapPublisherEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/publishers").WithTags("Publishers");

            // POST /api/publishers
            group.MapPost("/", async (CreatePublisherDto dto, LibraryDbContext db) =>
            {
                if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > 40)
                    return Results.BadRequest("Invalid publisher name.");

                var publisher = new Tpublishingcompany
                {
                    CName = dto.Name.Trim()
                };

                db.Tpublishingcompanies.Add(publisher);
                await db.SaveChangesAsync();

                return Results.Created($"/api/publishers/{publisher.NPublishingCompanyId}", Resource(publisher));
            }).WithName("CreatePublisher");

            // GET /api/publishers/{id}
            group.MapGet("/{id:int}", async (int id, LibraryDbContext db) =>
            {
                var publisher = await db.Tpublishingcompanies.AsNoTracking().FirstOrDefaultAsync(p => p.NPublishingCompanyId == id);

                return publisher is null
                    ? Results.NotFound()
                    : Results.Ok(Resource(publisher));
            }).WithName("GetPublisherById");


            // GET /api/publishers
            group.MapGet("/", async (LibraryDbContext db) =>
            {
                var publishers = await db.Tpublishingcompanies
                    .AsNoTracking()
                    .OrderBy(p => p.NPublishingCompanyId)
                    .ToListAsync();

                return Results.Ok(new
                {
                    Items = publishers.Select(Resource),
                    _links = new Dictionary<string, HateoasLink>
                    {
                        ["self"] = HateoasHelper.Link("/api/publishers", "GET", "List publishers"),
                        ["create"] = HateoasHelper.Link("/api/publishers", "POST", "Create a publisher")
                    }
                });
            }).WithName("GetPublishers");

            // PUT /api/publishers/{id}
            group.MapPut("/{id:int}", async (int id, UpdatePublisherDto dto, LibraryDbContext db) =>
            {
                if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > 40)
                    return Results.BadRequest("Invalid publisher name.");

                var publisher = await db.Tpublishingcompanies.FindAsync(id);
                if (publisher is null)
                    return Results.NotFound();

                publisher.CName = dto.Name.Trim();
                await db.SaveChangesAsync();

                return Results.Ok(Resource(publisher));
            }).WithName("UpdatePublisher");

            // DELETE /api/publishers/{id}
            group.MapDelete("/{id:int}", async (int id, LibraryDbContext db) =>
            {
                var publisher = await db.Tpublishingcompanies.FindAsync(id);
                if (publisher is null)
                    return Results.NotFound();

                if (await db.Tbooks.AnyAsync(
                    b => b.NPublishingCompanyId == id))
                    return Results.Conflict("Cannot delete a publisher that has books.");

                db.Tpublishingcompanies.Remove(publisher);
                await db.SaveChangesAsync();

                return Results.Ok(new
                {
                    Message = "Publisher deleted successfully.",
                    _links = new Dictionary<string, HateoasLink>
                    {
                        ["all-publishers"] = HateoasHelper.Link("/api/publishers", "GET", "List publishers"),
                        ["create"] = HateoasHelper.Link("/api/publishers", "POST", "Create a publisher")
                    }
                });
            }).WithName("DeletePublisher");
        }

        private static HateoasResource<PublisherDto> Resource(
            Tpublishingcompany publisher)
        {
            var dto = new PublisherDto(publisher.NPublishingCompanyId, publisher.CName);

            var links = new Dictionary<string, HateoasLink>
            {
                ["self"] = HateoasHelper.Link($"/api/publishers/{publisher.NPublishingCompanyId}", "GET", "Read publisher"),
                ["update"] = HateoasHelper.Link($"/api/publishers/{publisher.NPublishingCompanyId}", "PUT", "Update publisher"),
                ["delete"] = HateoasHelper.Link($"/api/publishers/{publisher.NPublishingCompanyId}", "DELETE", "Delete publisher"),
                ["all-publishers"] = HateoasHelper.Link("/api/publishers", "GET", "List publishers")
            };

            return HateoasHelper.Resource(dto, links);
        }
    }
}

using Library.Data.Context;
using Library.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApi.DTOs;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("api/publishers")]
    public class PublishersController : ControllerBase
    {
        private readonly LibraryDbContext _db;

        public PublishersController(LibraryDbContext db)
        {
            _db = db;
        }

        // POST: api/publishers
        [HttpPost]
        public async Task<IActionResult> CreatePublisher(
            CreatePublisherDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                dto.Name.Length > 40)
            {
                return BadRequest("Invalid publisher name.");
            }

            var publisher = new Tpublishingcompany
            {
                CName = dto.Name.Trim()
            };

            _db.Tpublishingcompanies.Add(publisher);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPublisherById),
                new { id = publisher.NPublishingCompanyId },
                Resource(publisher));
        }

        // GET: api/publishers/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPublisherById(int id)
        {
            var publisher = await _db.Tpublishingcompanies
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.NPublishingCompanyId == id);

            if (publisher == null)
                return NotFound();

            return Ok(Resource(publisher));
        }

        // GET: api/publishers
        [HttpGet]
        public async Task<IActionResult> GetPublishers()
        {
            var publishers = await _db.Tpublishingcompanies
                .AsNoTracking()
                .OrderBy(p => p.NPublishingCompanyId)
                .ToListAsync();

            return Ok(new
            {
                Items = publishers.Select(Resource),

                _links = new Dictionary<string, HateoasLink>
                {
                    ["self"] = HateoasHelper.Link(
                        "/api/publishers",
                        "GET",
                        "List publishers"),

                    ["create"] = HateoasHelper.Link(
                        "/api/publishers",
                        "POST",
                        "Create a publisher")
                }
            });
        }

        // PUT: api/publishers/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePublisher(
            int id,
            UpdatePublisherDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                dto.Name.Length > 40)
            {
                return BadRequest("Invalid publisher name.");
            }

            var publisher = await _db.Tpublishingcompanies
                .FindAsync(id);

            if (publisher == null)
                return NotFound();

            publisher.CName = dto.Name.Trim();

            await _db.SaveChangesAsync();

            return Ok(Resource(publisher));
        }

        // DELETE: api/publishers/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            var publisher = await _db.Tpublishingcompanies
                .FindAsync(id);

            if (publisher == null)
                return NotFound();

            // Prevent deleting publishers that have books
            if (await _db.Tbooks.AnyAsync(
                b => b.NPublishingCompanyId == id))
            {
                return Conflict(
                    "Cannot delete a publisher that has books.");
            }

            _db.Tpublishingcompanies.Remove(publisher);

            await _db.SaveChangesAsync();

            return Ok(new
            {
                Message = "Publisher deleted successfully.",

                _links = new Dictionary<string, HateoasLink>
                {
                    ["all-publishers"] = HateoasHelper.Link(
                        "/api/publishers",
                        "GET",
                        "List publishers"),

                    ["create"] = HateoasHelper.Link(
                        "/api/publishers",
                        "POST",
                        "Create a publisher")
                }
            });
        }

        // HATEOAS resource
        private static HateoasResource<PublisherDto> Resource(
            Tpublishingcompany publisher)
        {
            var dto = new PublisherDto(
                publisher.NPublishingCompanyId,
                publisher.CName);

            var links = new Dictionary<string, HateoasLink>
            {
                ["self"] = HateoasHelper.Link(
                    $"/api/publishers/{publisher.NPublishingCompanyId}",
                    "GET",
                    "Read publisher"),

                ["update"] = HateoasHelper.Link(
                    $"/api/publishers/{publisher.NPublishingCompanyId}",
                    "PUT",
                    "Update publisher"),

                ["delete"] = HateoasHelper.Link(
                    $"/api/publishers/{publisher.NPublishingCompanyId}",
                    "DELETE",
                    "Delete publisher"),

                ["all-publishers"] = HateoasHelper.Link(
                    "/api/publishers",
                    "GET",
                    "List publishers")
            };

            return HateoasHelper.Resource(dto, links);
        }
    }
}

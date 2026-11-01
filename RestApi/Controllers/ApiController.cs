using Microsoft.AspNetCore.Mvc;
using RestApi.DTOs;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetApiRoot()
        {
            var response = new ApiRootResource
            {
                Links = new Dictionary<string, HateoasLink>
                {
                    ["books"] = new(
                        "/api/books",
                        "GET",
                        "List all books"),

                    ["authors"] = new(
                        "/api/authors",
                        "GET",
                        "List all authors"),

                    ["publishers"] = new(
                        "/api/publishers",
                        "GET",
                        "List all publishers")
                }
            };

            return Ok(response);
        }
    }
}

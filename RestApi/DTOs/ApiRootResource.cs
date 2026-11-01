using System.Text.Json.Serialization;

namespace RestApi.DTOs
{
    public class ApiRootResource
    {
        [JsonPropertyName("_links")]
        public Dictionary<string, HateoasLink> Links { get; init; }
            = new();
    }
}

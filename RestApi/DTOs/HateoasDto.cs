using System.Text.Json.Serialization;

namespace RestApi.DTOs
{
    public record HateoasLink(string Href, string Method, string Rel);

    public class HateoasResource<T>
    {
        public T Data { get; init; } = default!;

        [JsonPropertyName("_links")]
        public Dictionary<string, HateoasLink> Links { get; init; } = new();
    }

    public class HateoasCollection<T>
    {
        public int TotalCount { get; init; }

        public int Limit { get; init; }

        public int Offset { get; init; }

        public List<HateoasResource<T>> Items { get; init; } = new();

        [JsonPropertyName("_links")]
        public Dictionary<string, HateoasLink> Links { get; init; } = new();
    }
}

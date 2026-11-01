using System.Text.Json.Serialization;

namespace RestApi.DTOs
{
    public class PagedResponseDto<T>
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public IEnumerable<HateoasResource<T>> Items { get; set; }
            = new List<HateoasResource<T>>();

        [JsonPropertyName("_links")]
        public Dictionary<string, HateoasLink> Links { get; set; }
            = new();
    }
}

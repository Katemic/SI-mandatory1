using RestApi.DTOs;

namespace RestApi.Endpoints
{
    public static class HateoasHelper
    {
        public static HateoasLink Link(string href, string method, string rel)
        {
            return new HateoasLink(href, method, rel);
        }

        public static HateoasResource<T> Resource<T>(T data, Dictionary<string, HateoasLink> links)
        {
            return new HateoasResource<T>
            {
                Data = data,
                Links = links
            };
        }
    }
}

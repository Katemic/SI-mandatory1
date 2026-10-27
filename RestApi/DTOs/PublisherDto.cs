using System.ComponentModel.DataAnnotations;

namespace RestApi.DTOs
{

    public class CreatePublisherDto
    {
        [Required, StringLength(40)]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdatePublisherDto
    {
        [Required, StringLength(40)]
        public string Name { get; set; } = string.Empty;
    }

    public record PublisherDto(int Id, string Name);

}

using System.ComponentModel.DataAnnotations;

namespace RestApi.DTOs
{
    public class CreateAuthorDto
    {
        [Required, StringLength(40)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(60)]
        public string Surname { get; set; } = string.Empty;
    }

    public class UpdateAuthorDto
    {
        [Required, StringLength(40)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(60)]
        public string Surname { get; set; } = string.Empty;
    }

    public record AuthorDto(int Id, string Name, string Surname);

}

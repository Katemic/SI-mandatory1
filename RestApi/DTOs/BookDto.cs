using System.ComponentModel.DataAnnotations;

namespace RestApi.DTOs
{
    public class CreateBookDto
    {
        [Required, StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int AuthorId { get; set; }

        [Range(1, int.MaxValue)]
        public int PublisherId { get; set; }

        public int PublishingYear { get; set; }
    }

    public class UpdateBookDto
    {
        [Required, StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int AuthorId { get; set; }

        [Range(1, int.MaxValue)]
        public int PublisherId { get; set; }

        public int PublishingYear { get; set; }
    }

    public record BookDto(
        int Id,
        string Title,
        int AuthorId,
        int PublisherId,
        int PublishingYear
    );
}

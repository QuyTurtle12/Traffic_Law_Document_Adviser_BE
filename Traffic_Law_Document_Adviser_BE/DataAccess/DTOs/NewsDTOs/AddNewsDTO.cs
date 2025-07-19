namespace DataAccess.DTOs.NewsDTOs
{
    public class AddNewsDTO : BaseNewsDTO
    {
        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
        public string? Author { get; set; }
        public string? ImageUrl { get; set; }
        public string? EmbeddedUrl { get; set; }
        public Guid? UserId { get; set; }
    }
}
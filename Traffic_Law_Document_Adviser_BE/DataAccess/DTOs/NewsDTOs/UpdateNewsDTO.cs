namespace DataAccess.DTOs.NewsDTOs
{
    public class UpdateNewsDTO : BaseNewsDTO
    {
        public Guid Id { get; set; }
        public string? Author { get; set; }
        public string? ImageUrl { get; set; }
        public string? EmbeddedUrl { get; set; }
    }
}
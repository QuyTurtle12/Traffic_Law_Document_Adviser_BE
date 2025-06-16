namespace DataAccess.DTOs.NewsDTOs
{
    public class GetNewsDTO : BaseNewsDTO
    {
        public Guid Id { get; set; }
        public DateTime PublishedDate { get; set; }
        public string? Author { get; set; }
        public string? ImageUrl { get; set; }
        public string? EmbeddedUrl { get; set; }
        public int EmbeddedNewsId { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
        public Guid? UserId { get; set; }
    }
}
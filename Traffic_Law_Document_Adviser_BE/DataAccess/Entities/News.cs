namespace DataAccess.Entities
{
    public class News : BaseEntity
    {
        public int embeddedNewsId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
        public string? Author { get; set; }
        public string? ImageUrl { get; set; }
        public string? EmbeddedUrl { get; set; }

        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }
    }
}

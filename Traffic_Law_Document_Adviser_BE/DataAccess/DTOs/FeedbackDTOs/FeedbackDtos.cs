namespace DataAccess.DTOs.FeedbackDTOs
{
    public class GetFeedbackDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ChatHistory { get; set; }
        public string? AIAnswer { get; set; }
        public string? Content { get; set; }
    }
    public class PostFeedbackDto
    {
        public Guid UserId { get; set; }
        public Guid ChatHistory { get; set; }
        public string? AIAnswer { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    }
    public class PutFeedbackDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ChatHistory { get; set; }
        public string? AIAnswer { get; set; }
        public string? Content { get; set; }
    }
}

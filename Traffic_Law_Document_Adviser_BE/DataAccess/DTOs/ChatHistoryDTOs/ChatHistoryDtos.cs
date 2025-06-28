using System.ComponentModel.DataAnnotations;

namespace DataAccess.DTOs.ChatHistoryDTOs
{
    public class PostChatHistoryDto
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string Question { get; set; }
        public string? Answer { get; set; }
    }
    public class GetChatHistoryDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

namespace DataAccess.Entities
{
    public class ChatHistory : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
    }
}

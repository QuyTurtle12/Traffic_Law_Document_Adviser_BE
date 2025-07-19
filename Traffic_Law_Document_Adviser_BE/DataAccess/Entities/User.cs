namespace DataAccess.Entities
{
    public class User : BaseEntity
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public int? Role { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Feedback>? Feedbacks { get; set; }
        public virtual ICollection<ChatHistory>? ChatHistories { get; set; }
        public virtual ICollection<News>? News { get; set; }
        public virtual ICollection<LawDocument>? LawDocuments { get; set; }
    }
}

namespace DataAccess.Entities
{
    public class Feedback : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ChatHistory { get; set; }
        public string? AIAnswer { get; set; }
        public string? Content { get; set; }

        public virtual User? User { get; set; }
        public virtual ChatHistory? ChatHistoryNavigation { get; set; }
    }
}

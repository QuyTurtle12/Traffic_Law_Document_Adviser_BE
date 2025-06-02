namespace DataAccess.Entities
{
    public class ChatDocument
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid LawDocumentId { get; set; }

        public virtual User? User { get; set; }
        public virtual LawDocument? LawDocument { get; set; }

        public ChatDocument()
        {
            Id = Guid.NewGuid();
        }
    }
}

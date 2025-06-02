namespace DataAccess.Entities
{
    public class DocumentTagMap
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public Guid DocumentTagId { get; set; }

        public virtual LawDocument? Document { get; set; }
        public virtual DocumentTag? Tag { get; set; }

        public DocumentTagMap()
        {
            Id = Guid.NewGuid();
        }
    }
}

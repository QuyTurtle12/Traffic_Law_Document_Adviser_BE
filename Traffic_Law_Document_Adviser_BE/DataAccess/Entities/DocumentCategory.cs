namespace DataAccess.Entities
{
    public class DocumentCategory : BaseEntity
    {
        public string? Name { get; set; }
        public virtual ICollection<LawDocument>? LawDocuments { get; set; }
    }
}

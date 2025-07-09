namespace DataAccess.Entities
{
    public class LawDocument : BaseEntity
    {
        public string? Title { get; set; }
        public string? DocumentCode { get; set; }
        public Guid? CategoryId { get; set; }
        public string? FilePath { get; set; }
        public string? LinkPath { get; set; }
        public bool ExpertVerification { get; set; } = false;

        public virtual ICollection<ChatDocument>? ChatDocuments { get; set; }
        public virtual DocumentCategory? Category { get; set; }
        public virtual ICollection<DocumentTagMap>? DocumentTagMaps { get; set; }
    }
}
 
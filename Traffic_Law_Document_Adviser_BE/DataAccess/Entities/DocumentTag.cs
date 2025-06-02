namespace DataAccess.Entities
{
    public class DocumentTag : BaseEntity
    {
        public string? Name { get; set; }
        public Guid? ParentTagId { get; set; }

        public virtual DocumentTag? ParentTag { get; set; }
        public virtual ICollection<DocumentTag>? ChildTags { get; set; }
        public virtual ICollection<DocumentTagMap>? DocumentTagMaps { get; set; }
    }
}

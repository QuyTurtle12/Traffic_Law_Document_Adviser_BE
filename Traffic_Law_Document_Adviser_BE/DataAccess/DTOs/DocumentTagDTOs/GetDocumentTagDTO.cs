namespace DataAccess.DTOs.DocumentTagDTOs
{
    public class GetDocumentTagDTO : BaseDocumentTagDTO
    {
        public Guid Id { get; set; }
        public Guid? ParentTagId { get; set; }
        public string? ParentTagName { get; set; }
        public ICollection<string>? ChildTagNames { get; set; }
    }
}

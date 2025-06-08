namespace DataAccess.DTOs.DocumentTagDTOs
{
    public class UpdateDocumentTagDTO : BaseDocumentTagDTO
    {
        public Guid? ParentTagId { get; set; }
    }
}

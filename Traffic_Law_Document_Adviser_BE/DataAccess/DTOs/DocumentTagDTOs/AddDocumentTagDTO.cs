namespace DataAccess.DTOs.DocumentTagDTOs
{
    public class AddDocumentTagDTO : BaseDocumentTagDTO
    {
        public Guid? ParentTagId { get; set; }
    }
}

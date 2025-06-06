namespace DataAccess.DTOs.DocumentTagMapDTOs
{
    public class GetDocumentTagMapDTO
    {
        public Guid Id { get; set; }
        public string TagName { get; set; } = string.Empty;
    }
}

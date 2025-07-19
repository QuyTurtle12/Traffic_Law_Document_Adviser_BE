using DataAccess.DTOs.DocumentTagMapDTOs;

namespace DataAccess.DTOs.LawDocumentDTOs
{
    public class GetLawDocumentDTO : BaseLawDocumentDTO
    {
        public Guid Id { get; set; }
        public string? CategoryName { get; set; }
        public IEnumerable<GetDocumentTagMapDTO>? TagList { get; set; }
        public bool ExpertVerification { get; set; }
        public string? VerifyBy { get; set; }
    }
}

using DataAccess.DTOs.DocumentTagMapDTOs;

namespace DataAccess.DTOs.LawDocumentDTOs
{
    public class AddLawDocumentDTO : BaseLawDocumentDTO
    {
        public bool ExpertVerification { get; set; } = false;
        public IEnumerable<AddDocumentTagMapDTO>? TagList { get; set; }
    }
}

using DataAccess.DTOs.DocumentTagMapDTOs;

namespace DataAccess.DTOs.LawDocumentDTOs
{
    public class UpdateLawDocumentDTO : BaseLawDocumentDTO
    {

        public bool ExpertVerification { get; set; }
        public IEnumerable<AddDocumentTagMapDTO>? TagList { get; set; }
    }
}

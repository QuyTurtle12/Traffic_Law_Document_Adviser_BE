using DataAccess.DTOs.DocumentTagMapDTOs;

namespace DataAccess.DTOs.LawDocumentDTOs
{
    public class UpdateLawDocumentDTO : BaseLawDocumentDTO
    {
        public IEnumerable<AddDocumentTagMapDTO>? TagList { get; set; }
    }
}

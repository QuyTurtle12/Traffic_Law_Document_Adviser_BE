using DataAccess.DTOs.DocumentTagMapDTOs;

namespace DataAccess.DTOs.LawDocumentDTOs
{
    public class AddLawDocumentDTO : BaseLawDocumentDTO
    {
        public IEnumerable<AddDocumentTagMapDTO>? TagList { get; set; }
    }
}

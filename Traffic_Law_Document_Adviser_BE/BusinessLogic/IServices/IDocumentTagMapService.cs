using DataAccess.DTOs.DocumentTagMapDTOs;

namespace BusinessLogic.IServices
{
    public interface IDocumentTagMapService
    {
        Task AddDocumentTagMapAsync(AddDocumentTagMapDTO documentTagMapDto);
        Task<IEnumerable<GetDocumentTagMapDTO>> GetDocumentTagMapsAsync(Guid documentId);
        Task DeleteDocumentTagMapAsync(Guid documentTagMapId);
    }
}

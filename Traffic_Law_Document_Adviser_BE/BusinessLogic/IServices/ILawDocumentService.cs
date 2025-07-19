using DataAccess.DTOs.LawDocumentDTOs;
using DataAccess.PaginatedList;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.IServices
{
    public interface ILawDocumentService
    {
        Task<PaginatedList<GetLawDocumentDTO>> GetPaginatedLawDocumentsAsync(int pageIndex, int pageSize, Guid? idSearch, string? titleSearch, string? documentCodeSearch, 
            string? categoryNameSearch, string? filePathSearch, string? linkPathSearch, bool? expertVerificationSearch, string[]? tagIdSearch);
        Task<GetLawDocumentDTO> GetLawDocumentById(Guid id);
        Task CreateLawDocument(AddLawDocumentDTO lawDocumentDTO);
        Task UpdateLawDocument(Guid id, UpdateLawDocumentDTO lawDocumentDTO);
        Task DeleteLawDocument(Guid id);
        Task SoftDeleteLawDocument(Guid id);
        Task VerifyDocument(Guid id);
        Task CreateLawDocumentWithUploadAsync(AddLawDocumentDTO dto, IFormFile file);

    }
}

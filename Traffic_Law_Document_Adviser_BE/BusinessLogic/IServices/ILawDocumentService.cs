using DataAccess.DTOs.LawDocumentDTOs;
using DataAccess.Entities;
using DataAccess.PaginatedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IServices
{
    public interface ILawDocumentService
    {
        Task<PaginatedList<GetLawDocumentDTO>> GetPaginatedLawDocumentsAsync(int pageIndex, int pageSize, Guid? idSearch, string? titleSearch, string? documentCodeSearch, 
            string? categoryNameSearch, string? filePathSearch, string? linkPathSearch, bool? expertVerificationSearch);
        Task<GetLawDocumentDTO> GetLawDocumentById(Guid id);
        Task CreateLawDocument(AddLawDocumentDTO lawDocumentDTO);
        Task UpdateLawDocument(Guid id, UpdateLawDocumentDTO lawDocumentDTO);
        Task DeleteLawDocument(Guid id);
        Task SoftDeleteLawDocument(Guid id);
    }
}

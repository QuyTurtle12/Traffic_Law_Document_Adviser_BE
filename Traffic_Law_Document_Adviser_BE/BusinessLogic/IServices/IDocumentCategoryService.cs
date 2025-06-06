using DataAccess.DTOs.DocumentCategoryDTOs;
using DataAccess.PaginatedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IServices
{
    public interface IDocumentCategoryService
    {
        Task<PaginatedList<GetDocumentCategoryDTO>> GetPaginatedDocumentCategoriesAsync(int pageIndex, int pageSize, Guid? idSearch, string? nameSearch);
        Task<GetDocumentCategoryDTO> GetDocumentCategoryById(Guid id);
        Task CreateDocumentCategory(AddDocumentCategoryDTO cartDTO);
        Task UpdateDocumentCategory(Guid id, UpdateDocumentCategoryDTO cartDTO);
        Task DeleteDocumentCategory(Guid id);
        Task SoftDeleteDocumentCategory(Guid id);
    }
}

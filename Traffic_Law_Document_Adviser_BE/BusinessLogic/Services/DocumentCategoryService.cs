using BusinessLogic.IServices;
using DataAccess.DTOs.DocumentCategoryDTOs;
using DataAccess.PaginatedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class DocumentCategoryService : IDocumentCategoryService
    {
        public Task CreateDocumentCategory(AddDocumentCategoryDTO cartDTO)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDocumentCategory(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<GetDocumentCategoryDTO> GetDocumentCategoryById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<GetDocumentCategoryDTO>> GetPaginatedDocumentCategoriesAsync(int pageIndex, int pageSize, Guid? idSearch, string? nameSearch)
        {
            throw new NotImplementedException();
        }

        public Task SoftDeleteDocumentCategory(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDocumentCategory(Guid id, UpdateDocumentCategoryDTO cartDTO)
        {
            throw new NotImplementedException();
        }
    }
}

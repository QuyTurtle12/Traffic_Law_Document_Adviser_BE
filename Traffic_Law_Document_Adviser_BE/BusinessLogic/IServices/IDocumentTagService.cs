using DataAccess.DTOs.DocumentTagDTOs;
using DataAccess.PaginatedList;

namespace BusinessLogic.IServices
{
    public interface IDocumentTagService
    {
        public Task<PaginatedList<GetDocumentTagDTO>?> GetPaginatedListAsync(int pageIndex, int pageSize, string? idSearch, string? nameSearch, string? parentNameSearch);
        public Task<GetDocumentTagDTO?> GetByIdAsync(Guid id);
        public Task AddAsync(AddDocumentTagDTO documentTagDto);
        public Task UpdateAsync(Guid id, UpdateDocumentTagDTO documentTagDto);
        public Task DeleteAsync(Guid id);
    }
}

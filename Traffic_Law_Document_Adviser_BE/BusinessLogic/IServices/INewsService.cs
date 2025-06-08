using DataAccess.DTOs.NewsDTOs;
using DataAccess.PaginatedList;

namespace DataAccess.IServices
{
    public interface INewsService
    {
        Task<PaginatedList<GetNewsDTO>> GetPaginatedNewsAsync(int pageIndex = 1, int pageSize = 10);
        Task<GetNewsDTO?> GetNewsByIdAsync(Guid id);
        Task<GetNewsDTO> CreateNewsAsync(AddNewsDTO addNewsDTO);
        Task<GetNewsDTO?> UpdateNewsAsync(Guid id, AddNewsDTO updateNewsDTO);
        Task<bool> DeleteNewsAsync(Guid id);
        Task<List<GetNewsDTO>> SyncNewsFromApiAsync();
    }
}
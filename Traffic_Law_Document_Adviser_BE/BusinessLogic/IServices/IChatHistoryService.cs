using DataAccess.DTOs.ChatHistoryDTOs;

namespace BusinessLogic.IServices
{
    public interface IChatHistoryService
    {
        public Task<IEnumerable<GetChatHistoryDto>> GetChatHistoryListAsync(Guid userId, int page, int pageSize);
        public Task<GetChatHistoryDto> GetChatHistoryAsync(Guid chatId);
        public Task<bool> DeleteChatHistoryAsync(Guid chatId);
        public Task<bool> CreateChatHistoryAsync(PostChatHistoryDto postChatHistoryDto);
    }
}

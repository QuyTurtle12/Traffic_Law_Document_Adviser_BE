using AutoMapper;
using BusinessLogic.IServices;
using DataAccess.DTOs.ChatHistoryDTOs;
using DataAccess.Entities;
using DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class ChatHistoryService : IChatHistoryService
    {
        private readonly IMapper _mapper;
        private readonly IUOW _unitOfWork;

        public ChatHistoryService(IMapper mapper, IUOW uow)
        {
            _mapper = mapper;
            _unitOfWork = uow;
        }
        public async Task<Guid> CreateChatHistoryAsync(PostChatHistoryDto postChatHistoryDto)
        {
            postChatHistoryDto.Answer = "Bot answer is not available yet.";
            ChatHistory newChatHistory = _mapper.Map<ChatHistory>(postChatHistoryDto);
            newChatHistory.CreatedTime = DateTime.Now;
            Guid id = newChatHistory.Id;

            try {
                await _unitOfWork.GetRepository<ChatHistory>().InsertAsync(newChatHistory);
                await _unitOfWork.SaveAsync();
            }catch (Exception)
            {
                return Guid.Empty;
            }
            return id;
        }
        public async Task<bool> DeleteChatHistoryAsync(Guid chatId)
        {
            ChatHistory? chatHistory = await _unitOfWork.GetRepository<ChatHistory>()
                .Entities
                .Where(c => Guid.Equals(c.Id, chatId))
                .FirstOrDefaultAsync();

            if (chatHistory == null) return false;

            _unitOfWork.GetRepository<ChatHistory>().Delete(chatHistory);
            await _unitOfWork.SaveAsync();
            return true;
        }
        public async Task<GetChatHistoryDto> GetChatHistoryAsync(Guid chatId)
        {
            ChatHistory? chatHistory = await _unitOfWork.GetRepository<ChatHistory>()
               .Entities
               .Where(c => Guid.Equals(c.Id, chatId))
               .FirstOrDefaultAsync();

            return chatHistory == null ? null : _mapper.Map<GetChatHistoryDto>(chatHistory);
        }
        public async Task<IEnumerable<GetChatHistoryDto>> GetChatHistoryListAsync(Guid userId, int page, int pageSize)
        {
            IEnumerable<ChatHistory> chatHistoryList = await _unitOfWork.GetRepository<ChatHistory>()
                .Entities
                .Where(c => Guid.Equals(c.UserId, userId))
                .OrderByDescending(c => c.CreatedTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                
            return _mapper.Map<IEnumerable<GetChatHistoryDto>>(chatHistoryList);
        }
    }
}

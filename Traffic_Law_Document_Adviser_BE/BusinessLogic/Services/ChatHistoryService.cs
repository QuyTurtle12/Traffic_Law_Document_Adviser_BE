using AutoMapper;
using BusinessLogic.IServices;
using DataAccess.DTOs.ChatHistoryDTOs;
using DataAccess.Entities;
using DataAccess.ExceptionCustom;
using DataAccess.IRepositories;
using Microsoft.AspNetCore.Http;
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
        public async Task CreateChatHistoryAsync(PostChatHistoryDto postChatHistoryDto)
        {
            ChatHistory newChatHistory = _mapper.Map<ChatHistory>(postChatHistoryDto);
            await _unitOfWork.GetRepository<ChatHistory>()
                .InsertAsync(newChatHistory);
            await _unitOfWork.SaveAsync();
        }
        public async Task DeleteChatHistoryAsync(Guid chatId)
        {
            ChatHistory chatHistory = await _unitOfWork.GetRepository<ChatHistory>()
                .Entities
                .Where(c => Guid.Equals(c.Id, chatId))
                .FirstOrDefaultAsync();

            if(chatHistory == null)
                throw new CoreException("Chat history not found or has been deleted.", "CHATHISTORY_NOT_FOUND", StatusCodes.Status404NotFound);

            _unitOfWork.GetRepository<ChatHistory>().Delete(chatHistory);
            await _unitOfWork.SaveAsync();
        }
        public async Task<GetChatHistoryDto> GetChatHistoryAsync(Guid chatId)
        {
            ChatHistory chatHistory = await _unitOfWork.GetRepository<ChatHistory>()
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

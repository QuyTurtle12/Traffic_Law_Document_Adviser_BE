using BusinessLogic.IServices;
using DataAccess.DTOs.ChatHistoryDTOs;
using Microsoft.AspNetCore.Mvc;

namespace Traffic_Law_Document_Adviser_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatHistoryController : ControllerBase
    {
        private readonly IChatHistoryService _chatHistoryService;
        public ChatHistoryController(IChatHistoryService chatHistoryService)
        {
            _chatHistoryService = chatHistoryService;
        }
        [HttpGet("search/{userId}")]
        public async Task<IActionResult> GetChatHistoryListAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            var chatHistoryList = await _chatHistoryService.GetChatHistoryListAsync(userId, page, pageSize);
            return Ok(chatHistoryList);
        }
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChatHistoryAsync(Guid chatId)
        {
            var chatHistory = await _chatHistoryService.GetChatHistoryAsync(chatId);
            if (chatHistory == null)
                return NotFound();
            return Ok(chatHistory);
        }
        [HttpPost]
        public async Task<IActionResult> CreateChatHistoryAsync([FromBody] PostChatHistoryDto postChatHistoryDto)
        {
            await _chatHistoryService.CreateChatHistoryAsync(postChatHistoryDto);
            return Ok("Create success");
        }
        [HttpDelete("{chatId}")]
        public async Task<IActionResult> DeleteChatHistoryAsync(Guid chatId)
        {
            await _chatHistoryService.DeleteChatHistoryAsync(chatId);
            return NoContent();
        }
    }
}

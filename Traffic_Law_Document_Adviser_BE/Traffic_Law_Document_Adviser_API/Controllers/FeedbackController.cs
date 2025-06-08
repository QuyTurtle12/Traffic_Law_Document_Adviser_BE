using BusinessLogic.IServices;
using DataAccess.DTOs.FeedbackDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Traffic_Law_Document_Adviser_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        [HttpGet("{feedbackId}")]
        public async Task<IActionResult> GetFeedbackAsync(Guid feedbackId)
        {
            var feedback = await _feedbackService.GetFeedbackAsync(feedbackId);
            if (feedback == null)
                return NotFound();
            return Ok(feedback);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllFeedbacksByUserIdAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            var feedbacks = await _feedbackService.GetFeedbacksByUserIdAsync(userId, page, pageSize);
            return Ok(feedbacks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedbackAsync([FromBody] PostFeedbackDto postFeedbackDto)
        {
            await _feedbackService.CreateFeedbackAsync(postFeedbackDto);
            return Ok("Create success");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFeedbackAsync([FromBody] PutFeedbackDto putFeedbackDto)
        {
            await _feedbackService.UpdateFeedbackAsync(putFeedbackDto);
            return NoContent();
        }

        [HttpDelete("{feedbackId}")]
        public async Task<IActionResult> DeleteFeedbackAsync(Guid feedbackId)
        {
            await _feedbackService.DeleteFeedbackAsync(feedbackId);
            return NoContent();
        }
    } 
}

using DataAccess.DTOs.FeedbackDTOs;

namespace BusinessLogic.IServices
{
    public interface IFeedbackService
    {
        public Task<GetFeedbackDto> GetFeedbackAsync(Guid feedbackId);
        public Task DeleteFeedbackAsync(Guid feedbackId);
        public Task<IEnumerable<GetFeedbackDto>> GetFeedbacksByUserIdAsync(Guid userId, int page, int pageSize);
        public Task UpdateFeedbackAsync(PutFeedbackDto putFeedbackDto);
        public Task CreateFeedbackAsync(PostFeedbackDto postFeedbackDto);
        public Task<IEnumerable<GetFeedbackDto>> GetAllFeedbacksAsync(int page, int pageSize);
    }
}

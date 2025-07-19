using DataAccess.DTOs.FeedbackDTOs;

namespace BusinessLogic.IServices
{
    public interface IFeedbackService
    {
        public Task<GetFeedbackDto> GetFeedbackAsync(Guid feedbackId);
        public Task<bool> DeleteFeedbackAsync(Guid feedbackId);
        public Task<IEnumerable<GetFeedbackDto>> GetFeedbacksByUserIdAsync(Guid userId, int page, int pageSize);
        public Task<bool> UpdateFeedbackAsync(PutFeedbackDto putFeedbackDto);
        public Task<bool> CreateFeedbackAsync(PostFeedbackDto postFeedbackDto);
        public Task<IEnumerable<GetFeedbackDto>> GetAllFeedbacksAsync(int page, int pageSize);
    }
}

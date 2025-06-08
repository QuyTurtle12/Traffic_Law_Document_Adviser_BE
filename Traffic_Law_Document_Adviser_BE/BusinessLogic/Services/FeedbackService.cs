using AutoMapper;
using BusinessLogic.IServices;
using DataAccess.DTOs.ChatHistoryDTOs;
using DataAccess.DTOs.FeedbackDTOs;
using DataAccess.Entities;
using DataAccess.ExceptionCustom;
using DataAccess.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IMapper _mapper;
        private readonly IUOW _unitOfWork;
        public FeedbackService(IMapper mapper, IUOW uow)
        {
                    _mapper = mapper;
                    _unitOfWork = uow;
        }
        public async Task CreateFeedbackAsync(PostFeedbackDto postFeedbackDto)
        {
            Feedback newFeedback = _mapper.Map<Feedback>(postFeedbackDto);
            await _unitOfWork.GetRepository<Feedback>().InsertAsync(newFeedback);
            await _unitOfWork.SaveAsync();
        }
        public async Task DeleteFeedbackAsync(Guid feedbackId)
        {
            Feedback feedback = await _unitOfWork.GetRepository<Feedback>()
                .Entities
                .Where(f => Guid.Equals(f.Id, feedbackId) && f.DeletedTime == null)
                .FirstOrDefaultAsync();

            if (feedback == null)
            {
                throw new CoreException("Feedback not found or has been deleted.", "FEEDBACK_NOT_FOUND", StatusCodes.Status404NotFound);
            }

            feedback.DeletedTime = DateTime.UtcNow;
            _unitOfWork.GetRepository<Feedback>().Update(feedback);
            await _unitOfWork.SaveAsync();
        }
        public async Task<IEnumerable<GetFeedbackDto>> GetFeedbacksByUserIdAsync(Guid userId, int page, int pageSize)
        {
            IEnumerable<Feedback> feedbackList = await _unitOfWork.GetRepository<Feedback>()
                .Entities
                .Where(f => Guid.Equals(f.UserId, userId) && f.DeletedTime == null)
                .OrderByDescending(f => f.CreatedTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetFeedbackDto>>(feedbackList);
        }
        public async Task<GetFeedbackDto> GetFeedbackAsync(Guid feedbackId)
        {
            Feedback feedback = await _unitOfWork.GetRepository<Feedback>()
                .Entities
                .Where(f => Guid.Equals(f.Id, feedbackId) && f.DeletedTime == null)
                .FirstOrDefaultAsync();
            
            return feedback != null ? _mapper.Map<GetFeedbackDto>(feedback) : null;
        }
        public async Task UpdateFeedbackAsync(PutFeedbackDto putFeedbackDto)
        {
            
            Feedback feedback = await _unitOfWork.GetRepository<Feedback>()
                .Entities
                .Where(f => Guid.Equals(f.Id, putFeedbackDto.Id) && f.DeletedTime == null)
                .FirstOrDefaultAsync();

            if (feedback == null) 
                throw new CoreException("Feedback not found or has been deleted.", "FEEDBACK_NOT_FOUND", StatusCodes.Status404NotFound); ;

            feedback = _mapper.Map(putFeedbackDto, feedback);
            _unitOfWork.GetRepository<Feedback>().Update(feedback);
            await _unitOfWork.SaveAsync();
        }
    }
}

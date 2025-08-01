﻿using AutoMapper;
using BusinessLogic.IServices;
using DataAccess.DTOs.FeedbackDTOs;
using DataAccess.Entities;
using DataAccess.IRepositories;
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
        public async Task<bool> CreateFeedbackAsync(PostFeedbackDto postFeedbackDto)
        {
            Feedback newFeedback = _mapper.Map<Feedback>(postFeedbackDto);
            await _unitOfWork.GetRepository<Feedback>().InsertAsync(newFeedback);
            await _unitOfWork.SaveAsync();
            return true;
        }
        public async Task<bool> DeleteFeedbackAsync(Guid feedbackId)
        {
            Feedback? feedback = await _unitOfWork.GetRepository<Feedback>()
                .Entities
                .Where(f => Guid.Equals(f.Id, feedbackId) && f.DeletedTime == null)
                .FirstOrDefaultAsync();

            if (feedback == null)
            {
                return false;
            }

            feedback.DeletedTime = DateTime.UtcNow;
            _unitOfWork.GetRepository<Feedback>().Update(feedback);
            await _unitOfWork.SaveAsync();
            return true;
        }
        public async Task<IEnumerable<GetFeedbackDto>> GetFeedbacksByUserIdAsync(Guid userId, int page, int pageSize)
        {
            IEnumerable<Feedback> feedbackList = await _unitOfWork.GetRepository<Feedback>()
                .Entities
                .Where(f => Guid.Equals(f.UserId, userId) && f.DeletedTime == null)
                .Include(f => f.User)
                .OrderByDescending(f => f.CreatedTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetFeedbackDto>>(feedbackList);
        }
        public async Task<GetFeedbackDto> GetFeedbackAsync(Guid feedbackId)
        {
            Feedback? feedback = await _unitOfWork.GetRepository<Feedback>()
                .Entities
                .Where(f => Guid.Equals(f.Id, feedbackId) && f.DeletedTime == null)
                .Include(f => f.User)
                .FirstOrDefaultAsync();
            
            return feedback != null ? _mapper.Map<GetFeedbackDto>(feedback) : null;
        }
        public async Task<bool> UpdateFeedbackAsync(PutFeedbackDto putFeedbackDto)
        {
            
            Feedback? feedback = await _unitOfWork.GetRepository<Feedback>()
                .Entities
                .Where(f => Guid.Equals(f.Id, putFeedbackDto.Id) && f.DeletedTime == null)
                .FirstOrDefaultAsync();

            if (feedback == null) return false;

            feedback = _mapper.Map(putFeedbackDto, feedback);
            _unitOfWork.GetRepository<Feedback>().Update(feedback);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<GetFeedbackDto>> GetAllFeedbacksAsync(int page, int pageSize)
        {
            IEnumerable<Feedback> feedbackList = await _unitOfWork.GetRepository<Feedback>()
                .Entities
                .Where(f => f.DeletedTime == null)
                .Include(f => f.User)
                .OrderByDescending(f => f.CreatedTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetFeedbackDto>>(feedbackList);
        }
    }
}

using AutoMapper;
using BusinessLogic.IServices;
using DataAccess.Constant;
using DataAccess.DTOs.LawDocumentDTOs;
using DataAccess.Entities;
using DataAccess.ExceptionCustom;
using DataAccess.IRepositories;
using DataAccess.PaginatedList;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class LawDocumentService : ILawDocumentService
    {
        private readonly IMapper _mapper;
        private readonly IUOW _unitOfWork;

        // Constructor
        public LawDocumentService(IMapper mapper, IUOW uow)
        {
            _mapper = mapper;
            _unitOfWork = uow;
        }

        public async Task<PaginatedList<GetLawDocumentDTO>> GetPaginatedLawDocumentsAsync(int pageIndex, int pageSize, Guid? idSearch, string? titleSearch, string? documentCodeSearch,
            string? categoryNameSearch, string? filePathSearch, string? linkPathSearch, bool? expertVerificationSearch)
        {
            if (pageIndex < 1 && pageSize < 1)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Page index or page size must be greater than or equal to 1.");
            }

            IQueryable<LawDocument> query = _unitOfWork.GetRepository<LawDocument>().Entities.Include(dc => dc.Category);

            // Apply id search filters if provided
            if (idSearch.HasValue)
            {
                query = query.Where(p => p.Id.Equals(idSearch.Value));
            }

            if (!titleSearch.IsNullOrEmpty())
            {
                query = query.Where(p => p.Title!.Contains(titleSearch!));
            }

            if (!documentCodeSearch.IsNullOrEmpty())
            {
                query = query.Where(p => p.DocumentCode!.Contains(documentCodeSearch!));
            }

            if (!categoryNameSearch.IsNullOrEmpty())
            {

                query = query.Where(p => p.Category!.Name!.Contains(categoryNameSearch!));
            }

            if (!filePathSearch.IsNullOrEmpty())
            {
                query = query.Where(p => p.FilePath!.Contains(filePathSearch!));
            }

            if (!linkPathSearch.IsNullOrEmpty())
            {
                query = query.Where(p => p.LinkPath!.Contains(linkPathSearch!));
            }

            if (expertVerificationSearch.HasValue)
            {
                query = query.Where(p => p.ExpertVerification == expertVerificationSearch);
            }

            query = query.OrderByDescending(p => p.CreatedTime);

            // Change to paginated list to facilitate mapping process
            PaginatedList<LawDocument> resultQuery = await _unitOfWork.GetRepository<LawDocument>()
                .GetPagging(query, pageIndex, pageSize);

            // Map the result to GetLawDocumentDTO
            IReadOnlyCollection<GetLawDocumentDTO> result = resultQuery.Items.Select(item =>
            {
                GetLawDocumentDTO lawDocumentDTODTO = _mapper.Map<GetLawDocumentDTO>(item);

                return lawDocumentDTODTO;
            }).ToList();

            PaginatedList<GetLawDocumentDTO> paginatedList = new PaginatedList<GetLawDocumentDTO>(result, resultQuery.TotalCount, resultQuery.PageNumber, resultQuery.PageSize);

            return paginatedList;
        }

        public async Task<GetLawDocumentDTO> GetLawDocumentById(Guid id)
        {
            LawDocument? lawDocument = await _unitOfWork.GetRepository<LawDocument>().GetByIdAsync(id);
            if (lawDocument == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Law document not found!");
            }
            GetLawDocumentDTO responseItem = _mapper.Map<GetLawDocumentDTO>(lawDocument);
            return responseItem;
        }

        public async Task CreateLawDocument(AddLawDocumentDTO lawDocumentDTO)
        {
            if (lawDocumentDTO == null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Law document data is required!");
            }

            LawDocument lawDocument = _mapper.Map<LawDocument>(lawDocumentDTO);
            lawDocument.CreatedBy = "System";
            lawDocument.CreatedTime = DateTime.Now;

            await _unitOfWork.GetRepository<LawDocument>().InsertAsync(lawDocument);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateLawDocument(Guid id, UpdateLawDocumentDTO lawDocumentDTO)
        {
            IGenericRepository<LawDocument> repository = _unitOfWork.GetRepository<LawDocument>();
            LawDocument? existingLawDocument = await repository.GetByIdAsync(id);
            if (existingLawDocument == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.BADREQUEST, "Law Document not found!");
            }

            LawDocument lawDocument = _mapper.Map(lawDocumentDTO, existingLawDocument);
            lawDocument.LastUpdatedBy = "System";
            lawDocument.LastUpdatedTime = DateTime.Now;

            repository.Update(existingLawDocument);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteLawDocument(Guid id)
        {
            IGenericRepository<LawDocument> repository = _unitOfWork.GetRepository<LawDocument>();
            LawDocument? existingLawDocument = await repository.GetByIdAsync(id);
            if (existingLawDocument == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.BADREQUEST, "Law Document not found!");
            }

            repository.Delete(existingLawDocument);
            await _unitOfWork.SaveAsync();
        }

        public async Task SoftDeleteLawDocument(Guid id)
        {
            IGenericRepository<LawDocument> repository = _unitOfWork.GetRepository<LawDocument>();
            LawDocument? existingLawDocument = await repository.GetByIdAsync(id);
            if (existingLawDocument == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.BADREQUEST, "Law document not found!");
            }
            existingLawDocument.DeletedBy = "System";
            existingLawDocument.DeletedTime = DateTime.Now;
            existingLawDocument.LastUpdatedBy = "System";
            existingLawDocument.LastUpdatedTime = DateTime.Now;

            repository.Update(existingLawDocument);
            await _unitOfWork.SaveAsync();
        }

        
    }
}

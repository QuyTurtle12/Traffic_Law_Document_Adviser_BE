using AutoMapper;
using BusinessLogic.IServices;
using DataAccess.Constant;
using DataAccess.DTOs.DocumentCategoryDTOs;
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
    public class DocumentCategoryService : IDocumentCategoryService
    {
        private readonly IMapper _mapper;
        private readonly IUOW _unitOfWork;

        // Constructor
        public DocumentCategoryService(IMapper mapper, IUOW uow)
        {
            _mapper = mapper;
            _unitOfWork = uow;
        }

        public async Task<PaginatedList<GetDocumentCategoryDTO>> GetPaginatedDocumentCategoriesAsync(int pageIndex, int pageSize, Guid? idSearch, string? nameSearch)
        {
            if (pageIndex < 1 && pageSize < 1)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Page index or page size must be greater than or equal to 1.");
            }

            IQueryable<DocumentCategory> query = _unitOfWork.GetRepository<DocumentCategory>().Entities.Where(dc => !dc.DeletedTime.HasValue);

            // Apply id search filters if provided
            if (idSearch.HasValue)
            {
                query = query.Where(p => p.Id.Equals(idSearch.Value));
            }

            if (!nameSearch.IsNullOrEmpty())
            {
                query = query.Where(p => p.Name!.Contains(nameSearch!));
            }

            query = query.OrderByDescending(p => p.CreatedTime);

            // Change to paginated list to facilitate mapping process
            PaginatedList<DocumentCategory> resultQuery = await _unitOfWork.GetRepository<DocumentCategory>()
                .GetPagging(query, pageIndex, pageSize);

            // Map the result to GetDocumentCategoryDTO
            IReadOnlyCollection<GetDocumentCategoryDTO> result = resultQuery.Items.Select(item =>
            {
                GetDocumentCategoryDTO DocumentCategoryDTODTO = _mapper.Map<GetDocumentCategoryDTO>(item);

                return DocumentCategoryDTODTO;
            }).ToList();

            PaginatedList<GetDocumentCategoryDTO> paginatedList = new PaginatedList<GetDocumentCategoryDTO>(result, resultQuery.TotalCount, resultQuery.PageNumber, resultQuery.PageSize);

            return paginatedList;
        }

        public async Task<GetDocumentCategoryDTO> GetDocumentCategoryById(Guid id)
        {
            DocumentCategory? DocumentCategory = await _unitOfWork.GetRepository<DocumentCategory>().GetByIdAsync(id);
            if (DocumentCategory == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Document Category not found!");
            }
            GetDocumentCategoryDTO responseItem = _mapper.Map<GetDocumentCategoryDTO>(DocumentCategory);
            return responseItem;
        }

        public async Task CreateDocumentCategory(AddDocumentCategoryDTO documentCategoryDTO)
        {
            if (documentCategoryDTO == null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Document Category data is required!");
            }

            bool isCategoryNameExist = await _unitOfWork.GetRepository<DocumentCategory>()
                 .Entities
                 .AnyAsync(dc =>
                 EF.Functions.Collate(dc.Name!, "Latin1_General_CS_AS") == documentCategoryDTO.Name &&
                 !dc.DeletedTime.HasValue);

            if (isCategoryNameExist)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.EXISTED,
                    $"A document category with name '{documentCategoryDTO.Name}' already exists.");
            }

            DocumentCategory DocumentCategory = _mapper.Map<DocumentCategory>(documentCategoryDTO);
            DocumentCategory.CreatedBy = "System";
            DocumentCategory.CreatedTime = DateTime.Now;

            await _unitOfWork.GetRepository<DocumentCategory>().InsertAsync(DocumentCategory);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateDocumentCategory(Guid id, UpdateDocumentCategoryDTO documentCategoryDTO)
        {
            IGenericRepository<DocumentCategory> repository = _unitOfWork.GetRepository<DocumentCategory>();
            DocumentCategory? existingDocumentCategory = await repository.GetByIdAsync(id);
            if (existingDocumentCategory == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.BADREQUEST, "Document Category not found!");
            }

            bool isCategoryNameExist = await _unitOfWork.GetRepository<DocumentCategory>()
                 .Entities
                 .AnyAsync(dc =>
                 dc.Id != id &&
                 EF.Functions.Collate(dc.Name!, "Latin1_General_CS_AS") == documentCategoryDTO.Name &&
                 !dc.DeletedTime.HasValue);

            if (isCategoryNameExist)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.EXISTED,
                    $"A document category with name '{documentCategoryDTO.Name}' already exists.");
            }

            DocumentCategory DocumentCategory = _mapper.Map(documentCategoryDTO, existingDocumentCategory);
            DocumentCategory.LastUpdatedBy = "System";
            DocumentCategory.LastUpdatedTime = DateTime.Now;

            repository.Update(existingDocumentCategory);
            await _unitOfWork.SaveAsync();
        }

        public Task DeleteDocumentCategory(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task SoftDeleteDocumentCategory(Guid id)
        {
            IGenericRepository<DocumentCategory> repository = _unitOfWork.GetRepository<DocumentCategory>();
            DocumentCategory? existingDocumentCategory = await repository.GetByIdAsync(id);
            if (existingDocumentCategory == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.BADREQUEST, "Document Category not found!");
            }
            existingDocumentCategory.DeletedBy = "System";
            existingDocumentCategory.DeletedTime = DateTime.Now;
            existingDocumentCategory.LastUpdatedBy = "System";
            existingDocumentCategory.LastUpdatedTime = DateTime.Now;

            repository.Update(existingDocumentCategory);
            await _unitOfWork.SaveAsync();
        }

        
    }
}

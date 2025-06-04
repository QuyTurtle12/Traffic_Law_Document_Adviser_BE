using AutoMapper;
using BusinessLogic.IServices;
using DataAccess.Constant;
using DataAccess.DTOs.DocumentTagDTOs;
using DataAccess.Entities;
using DataAccess.ExceptionCustom;
using DataAccess.IRepositories;
using DataAccess.PaginatedList;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class DocumentTagService : IDocumentTagService
    {
        private readonly IMapper _mapper;
        private readonly IUOW _unitOfWork;

        // Constructor
        public DocumentTagService(IMapper mapper, IUOW uow)
        {
            _mapper = mapper;
            _unitOfWork = uow;
        }

        public async Task<PaginatedList<GetDocumentTagDTO>?> GetPaginatedListAsync(int pageIndex, int pageSize, string? idSearch, string? nameSearch, string? parentNameSearch)
        {
            // Validate page parameters
            if (pageIndex < 1 || pageSize < 1)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Page index and page size must be greater than or equal to 1.");
            }

            // Get all DocumentTag included ParentTag and ChildTags but excluded deleted data
            IQueryable<DocumentTag> query = _unitOfWork.GetRepository<DocumentTag>().Entities
                .Where(dt => !dt.DeletedTime.HasValue)
                .Include(dt => dt.ParentTag)
                .Include(dt => dt.ChildTags);

            // Apply id search filters if provided
            if (!string.IsNullOrEmpty(idSearch))
            {
                query = query.Where(dt => dt.Id.Equals(Guid.Parse(idSearch)));
            }

            // Apply name search filters if provided
            if (!string.IsNullOrEmpty(nameSearch))
            {
                query = query.Where(dt => dt.Name!.Contains(nameSearch));
            }

            // Apply parent name search filters if provided
            if (!string.IsNullOrEmpty(parentNameSearch))
            {
                query = query.Where(dt => dt.ParentTag != null && dt.ParentTag.Name!.Contains(parentNameSearch));
            }

            // Sort the query by Name
            query = query.OrderBy(dt => dt.Name);

            // Change to paginated list to facilitate mapping process
            PaginatedList<DocumentTag> resultQuery = await _unitOfWork.GetRepository<DocumentTag>()
                .GetPagging(query, pageIndex, pageSize);

            // Map the result to GetDocumentTagDTO
            IReadOnlyCollection<GetDocumentTagDTO> result = resultQuery.Items.Select(item =>
            {
                GetDocumentTagDTO documentTagDTO = _mapper.Map<GetDocumentTagDTO>(item);

                // Set ParentTagName
                documentTagDTO.ParentTagName = item.ParentTag?.Name ?? string.Empty;

                // Set child tag names, excluding deleted ones
                documentTagDTO.ChildTagNames = item.ChildTags?
                    .Where(child => !child.DeletedTime.HasValue)
                    .Select(child => child.Name ?? string.Empty)
                    .ToList();

                return documentTagDTO;
            }).ToList();

            return new PaginatedList<GetDocumentTagDTO>(result, resultQuery.TotalCount, pageIndex, pageSize);
        }

        public async Task<GetDocumentTagDTO?> GetByIdAsync(Guid id)
        {
            // Get the document tag by id, including its parent tag, but excluding deleted data
            DocumentTag? documentTag = await _unitOfWork.GetRepository<DocumentTag>().Entities
                .Where(dt => !dt.DeletedTime.HasValue)
                .Include(dt => dt.ParentTag)
                .Include(dt => dt.ChildTags)
                .FirstOrDefaultAsync(dt => dt.Id == id);

            // If the document tag is not found, throw error
            if (documentTag == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, $"Document tag with ID {id} not found");
            }

            // Map the DocumentTag entity to GetDocumentTagDTO
            GetDocumentTagDTO documentTagDTO = _mapper.Map<GetDocumentTagDTO>(documentTag);

            // Set ParentTagName
            documentTagDTO.ParentTagName = documentTag.ParentTag?.Name ?? string.Empty;

            // Set child tag names, excluding deleted ones
            documentTagDTO.ChildTagNames = documentTag.ChildTags?
                .Where(child => !child.DeletedTime.HasValue)
                .Select(child => child.Name ?? string.Empty)
                .ToList();

            return documentTagDTO;
        }

        public async Task AddAsync(AddDocumentTagDTO documentTagDto)
        {
            // Map the DTO to DocumentTag entity
            DocumentTag documentTag = _mapper.Map<DocumentTag>(documentTagDto);

            // Update audit fields
            documentTag.CreatedTime = DateTime.UtcNow;
            documentTag.LastUpdatedTime = documentTag.CreatedTime;

            // Add the new document tag to the repository
            await _unitOfWork.GetRepository<DocumentTag>().InsertAsync(documentTag);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateDocumentTagDTO documentTagDto)
        {
            // Get the existing document tag by id
            DocumentTag? existingDocumentTag = await _unitOfWork.GetRepository<DocumentTag>().GetByIdAsync(id);

            // If the document tag is not found, throw an exception
            if (existingDocumentTag == null || existingDocumentTag.DeletedTime.HasValue)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, $"Document tag with id {id} was not found");
            }

            // Map the updated values from the DTO to the existing entity
            _mapper.Map(documentTagDto, existingDocumentTag);

            // Update audit field
            existingDocumentTag.LastUpdatedTime = DateTime.UtcNow;

            // Update the document tag in the repository
            await _unitOfWork.GetRepository<DocumentTag>().UpdateAsync(existingDocumentTag);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            // Get the document tag by id
            DocumentTag? documentTag = await _unitOfWork.GetRepository<DocumentTag>().GetByIdAsync(id);
            
            // If the document tag is not found, throw an exception
            if (documentTag == null || documentTag.DeletedTime.HasValue)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, $"Document tag with id {id} was not found");
            }
            
            // Soft delete
            documentTag.DeletedTime = DateTime.UtcNow;
            documentTag.LastUpdatedTime = documentTag.DeletedTime;

            await _unitOfWork.SaveAsync();
        }
    }
}

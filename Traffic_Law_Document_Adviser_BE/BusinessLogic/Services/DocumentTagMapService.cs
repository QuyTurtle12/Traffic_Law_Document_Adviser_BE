using AutoMapper;
using BusinessLogic.IServices;
using DataAccess.DTOs.DocumentTagMapDTOs;
using DataAccess.Entities;
using DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class DocumentTagMapService : IDocumentTagMapService
    {
        private readonly IUOW _unitOfWork;
        private readonly IMapper _mapper;

        public DocumentTagMapService(IUOW unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddDocumentTagMapAsync(AddDocumentTagMapDTO documentTagMapDto)
        {
            // Map DTO to Entity
            var documentTagMap = _mapper.Map<DocumentTagMap>(documentTagMapDto);

            // Insert the new DocumentTagMap into the database
            await _unitOfWork.GetRepository<DocumentTagMap>().InsertAsync(documentTagMap);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<GetDocumentTagMapDTO>> GetDocumentTagMapsAsync(Guid documentId)
        {
            // Fetch all DocumentTagMaps for the given documentId, including related Tag entities
            IEnumerable<DocumentTagMap> documentTagMaps = await _unitOfWork.GetRepository<DocumentTagMap>().Entities
                .Where(dtm => dtm.DocumentId.Equals(documentId))
                .Include(dtm => dtm.Tag)
                .ToListAsync();

            // Map the entities to DTOs and include Tag names
            IEnumerable<GetDocumentTagMapDTO> result = documentTagMaps.Select(item =>
            {
                GetDocumentTagMapDTO dto = _mapper.Map<GetDocumentTagMapDTO>(item);
                dto.TagName = item.Tag?.Name ?? string.Empty;
                return dto;
            });

            return result;
        }

        public async Task DeleteDocumentTagMapAsync(Guid documentTagMapId)
        {
            // Delete the DocumentTagMap by its ID
            await _unitOfWork.GetRepository<DocumentTagMap>().DeleteAsync(documentTagMapId);
            await _unitOfWork.SaveAsync();
        }
    }
}

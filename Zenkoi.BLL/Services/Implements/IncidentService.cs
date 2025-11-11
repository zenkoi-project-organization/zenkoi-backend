using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.IncidentDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class IncidentService : IIncidentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<Incident> _incidentRepo;
        private readonly IRepoBase<IncidentType> _incidentTypeRepo;
        private readonly IRepoBase<KoiIncident> _koiIncidentRepo;
        private readonly IRepoBase<PondIncident> _pondIncidentRepo;
        private readonly IRepoBase<KoiFish> _koiFishRepo;
        private readonly IRepoBase<Pond> _pondRepo;

        public IncidentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _incidentRepo = _unitOfWork.GetRepo<Incident>();
            _incidentTypeRepo = _unitOfWork.GetRepo<IncidentType>();
            _koiIncidentRepo = _unitOfWork.GetRepo<KoiIncident>();
            _pondIncidentRepo = _unitOfWork.GetRepo<PondIncident>();
            _koiFishRepo = _unitOfWork.GetRepo<KoiFish>();
            _pondRepo = _unitOfWork.GetRepo<Pond>();
        }

        public async Task<PaginatedList<IncidentResponseDTO>> GetAllIncidentsAsync(
            IncidentFilterDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            filter ??= new IncidentFilterDTO();

            var queryBuilder = BuildIncidentQueryBuilder()
                .WithOrderBy(q => q.OrderByDescending(i => i.CreatedAt));

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var keyword = filter.Search.Trim();
                queryBuilder.WithPredicate(i =>
                    i.IncidentTitle.Contains(keyword) ||
                    (i.Description != null && i.Description.Contains(keyword)));
            }

            if (filter.IncidentTypeId.HasValue)
            {
                queryBuilder.WithPredicate(i => i.IncidentTypeId == filter.IncidentTypeId.Value);
            }

            if (filter.Severity.HasValue)
            {
                queryBuilder.WithPredicate(i => i.Severity == filter.Severity.Value);
            }

            if (filter.Status.HasValue)
            {
                queryBuilder.WithPredicate(i => i.Status == filter.Status.Value);
            }

            if (filter.ReportedByUserId.HasValue)
            {
                queryBuilder.WithPredicate(i => i.ReportedByUserId == filter.ReportedByUserId.Value);
            }

            if (filter.OccurredFrom.HasValue)
            {
                queryBuilder.WithPredicate(i => i.OccurredAt >= filter.OccurredFrom.Value);
            }

            if (filter.OccurredTo.HasValue)
            {
                queryBuilder.WithPredicate(i => i.OccurredAt <= filter.OccurredTo.Value);
            }

            var query = _incidentRepo.Get(queryBuilder.Build());
            var paginatedEntities = await PaginatedList<Incident>.CreateAsync(query, pageIndex, pageSize);
            var dtoList = _mapper.Map<List<IncidentResponseDTO>>(paginatedEntities);

            return new PaginatedList<IncidentResponseDTO>(
                dtoList,
                paginatedEntities.TotalItems,
                pageIndex,
                pageSize);
        }

        public async Task<IncidentResponseDTO> GetByIdAsync(int id)
        {
            var incident = await _incidentRepo.GetSingleAsync(
                BuildIncidentQueryBuilder()
                    .WithPredicate(i => i.Id == id)
                    .Build());

            if (incident == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy sự cố với id {id}.");
            }

            return _mapper.Map<IncidentResponseDTO>(incident);
        }

        public async Task<IncidentResponseDTO> CreateAsync(int userId, IncidentRequestDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var incidentTypeExists = await _incidentTypeRepo.CheckExistAsync(dto.IncidentTypeId);
            if (!incidentTypeExists)
            {
                throw new ArgumentException($"Không tìm thấy loại sự cố với id {dto.IncidentTypeId}.");
            }

            var incident = _mapper.Map<Incident>(dto);
            incident.ReportedByUserId = userId;
            incident.Status = IncidentStatus.Reported;
            incident.CreatedAt = DateTime.UtcNow;
            incident.UpdatedAt = null;
            incident.ResolvedAt = null;

            await _incidentRepo.CreateAsync(incident);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(incident.Id);
        }

        public async Task<IncidentResponseDTO> UpdateAsync(int id, IncidentUpdateRequestDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var incident = await _incidentRepo.GetByIdAsync(id);
            if (incident == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy sự cố với id {id}.");
            }

            if (dto.IncidentTypeId.HasValue)
            {
                var incidentTypeExists = await _incidentTypeRepo.CheckExistAsync(dto.IncidentTypeId.Value);
                if (!incidentTypeExists)
                {
                    throw new ArgumentException($"Không tìm thấy loại sự cố với id {dto.IncidentTypeId.Value}.");
                }
            }

            _mapper.Map(dto, incident);
            incident.UpdatedAt = DateTime.UtcNow;

            await _incidentRepo.UpdateAsync(incident);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var incident = await _incidentRepo.GetByIdAsync(id);
            if (incident == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy sự cố với id {id}.");
            }

            await _incidentRepo.DeleteAsync(incident);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeStatusAsync(int id, UpdateStatusDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var incident = await _incidentRepo.GetByIdAsync(id);
            if (incident == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy sự cố với id {id}.");
            }

            incident.Status = dto.Status;
            if (dto.Status == IncidentStatus.Resolved && !incident.ResolvedAt.HasValue)
            {
                incident.ResolvedAt = DateTime.UtcNow;
            }
            incident.UpdatedAt = DateTime.UtcNow;

            await _incidentRepo.UpdateAsync(incident);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResolveAsync(int id, int userId, ResolveIncidentDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var incident = await _incidentRepo.GetByIdAsync(id);
            if (incident == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy sự cố với id {id}.");
            }

            incident.Status = IncidentStatus.Resolved;
            incident.ResolvedAt = DateTime.UtcNow;
            incident.ResolvedByUserId = userId;
            incident.ResolutionNotes = dto.ResolutionNotes;
            incident.UpdatedAt = DateTime.UtcNow;

            await _incidentRepo.UpdateAsync(incident);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<KoiIncidentSimpleDTO> AddKoiIncidentAsync(int incidentId, KoiIncidentRequestDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var incidentExists = await _incidentRepo.CheckExistAsync(incidentId);
            if (!incidentExists)
            {
                throw new KeyNotFoundException($"Không tìm thấy sự cố với id {incidentId}.");
            }

            var koiFish = await _koiFishRepo.GetByIdAsync(dto.KoiFishId);
            if (koiFish == null)
            {
                throw new ArgumentException($"Không tìm thấy cá Koi với id {dto.KoiFishId}.");
            }

            var koiIncident = _mapper.Map<KoiIncident>(dto);
            koiIncident.IncidentId = incidentId;

            await _koiIncidentRepo.CreateAsync(koiIncident);
            await _unitOfWork.SaveChangesAsync();

            var created = await _koiIncidentRepo.GetSingleAsync(new QueryBuilder<KoiIncident>()
                .WithPredicate(ki => ki.Id == koiIncident.Id)
                .WithTracking(false)
                .WithInclude(ki => ki.KoiFish)
                .Build());

            if (created == null)
            {
                throw new InvalidOperationException("Không thể tải thông tin cá bị ảnh hưởng vừa tạo.");
            }

            return _mapper.Map<KoiIncidentSimpleDTO>(created);
        }

        public async Task<bool> RemoveKoiIncidentAsync(int koiIncidentId)
        {
            var koiIncident = await _koiIncidentRepo.GetByIdAsync(koiIncidentId);
            if (koiIncident == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy bản ghi cá bị ảnh hưởng với id {koiIncidentId}.");
            }

            await _koiIncidentRepo.DeleteAsync(koiIncident);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<PondIncidentSimpleDTO> AddPondIncidentAsync(int incidentId, PondIncidentRequestDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var incidentExists = await _incidentRepo.CheckExistAsync(incidentId);
            if (!incidentExists)
            {
                throw new KeyNotFoundException($"Không tìm thấy sự cố với id {incidentId}.");
            }

            var pond = await _pondRepo.GetByIdAsync(dto.PondId);
            if (pond == null)
            {
                throw new ArgumentException($"Không tìm thấy ao với id {dto.PondId}.");
            }

            var pondIncident = _mapper.Map<PondIncident>(dto);
            pondIncident.IncidentId = incidentId;

            await _pondIncidentRepo.CreateAsync(pondIncident);
            await _unitOfWork.SaveChangesAsync();

            var created = await _pondIncidentRepo.GetSingleAsync(new QueryBuilder<PondIncident>()
                .WithPredicate(pi => pi.Id == pondIncident.Id)
                .WithTracking(false)
                .WithInclude(pi => pi.Pond)
                .Build());

            if (created == null)
            {
                throw new InvalidOperationException("Không thể tải thông tin ao bị ảnh hưởng vừa tạo.");
            }

            return _mapper.Map<PondIncidentSimpleDTO>(created);
        }

        public async Task<bool> RemovePondIncidentAsync(int pondIncidentId)
        {
            var pondIncident = await _pondIncidentRepo.GetByIdAsync(pondIncidentId);
            if (pondIncident == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy bản ghi ao bị ảnh hưởng với id {pondIncidentId}.");
            }

            await _pondIncidentRepo.DeleteAsync(pondIncident);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private QueryBuilder<Incident> BuildIncidentQueryBuilder(bool tracked = false)
        {
            return new QueryBuilder<Incident>()
                .WithTracking(tracked)
                .WithInclude(i => i.IncidentType)
                .WithInclude(i => i.ReportedBy)
                .WithInclude(i => i.ResolvedBy)
                .WithInclude(i => i.KoiIncidents)
                .WithInclude(i => i.PondIncidents)             
                ;
        }
    }
}

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

            if (filter.PondId.HasValue)
            {
                queryBuilder.WithPredicate(i => i.PondIncidents.Any(pi => pi.PondId == filter.PondId.Value));
            }

            if (filter.KoiFishId.HasValue)
            {
                queryBuilder.WithPredicate(i => i.KoiIncidents.Any(ki => ki.KoiFishId == filter.KoiFishId.Value));
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

        public async Task<IncidentResponseDTO> CreateIncidentWithDetailsAsync(int userId, CreateIncidentWithDetailsDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var incidentType = await _incidentTypeRepo.GetByIdAsync(dto.IncidentTypeId);
                if (incidentType == null)
                {
                    throw new ArgumentException($"Không tìm thấy loại sự cố với id {dto.IncidentTypeId}.");
                }

                var incident = _mapper.Map<Incident>(dto);
                incident.ReportedByUserId = userId;

                await _incidentRepo.CreateAsync(incident);
                await _unitOfWork.SaveChangesAsync();

                // Add affected Koi fish
                if (dto.AffectedKoiFish != null && dto.AffectedKoiFish.Any())
                {
                    foreach (var koiDto in dto.AffectedKoiFish)
                    {
                        var koiFish = await _koiFishRepo.GetByIdAsync(koiDto.KoiFishId);
                        if (koiFish == null)
                        {
                            throw new ArgumentException($"Không tìm thấy cá Koi với id {koiDto.KoiFishId}.");
                        }

                        var koiIncident = _mapper.Map<KoiIncident>(koiDto);
                        koiIncident.IncidentId = incident.Id;

                        await _koiIncidentRepo.CreateAsync(koiIncident);

                        // Update KoiFish HealthStatus
                        koiFish.HealthStatus = koiDto.AffectedStatus;
                        koiFish.UpdatedAt = DateTime.UtcNow;
                        await _koiFishRepo.UpdateAsync(koiFish);
                    }
                }

                if (dto.AffectedPonds != null && dto.AffectedPonds.Any())
                {
                    foreach (var pondDto in dto.AffectedPonds)
                    {
                        var pond = await _pondRepo.GetByIdAsync(pondDto.PondId);
                        if (pond == null)
                        {
                            throw new ArgumentException($"Không tìm thấy ao với id {pondDto.PondId}.");
                        }

                        var pondIncident = _mapper.Map<PondIncident>(pondDto);
                        pondIncident.IncidentId = incident.Id;

                        await _pondIncidentRepo.CreateAsync(pondIncident);

                        pond.PondStatus = PondStatus.Maintenance;
                        await _pondRepo.UpdateAsync(pond);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return await GetByIdAsync(incident.Id);
            }
            catch
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<IncidentResponseDTO> UpdateAsync(int id, int userId, IncidentUpdateRequestDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var incident = await _incidentRepo.GetSingleAsync(
                    new QueryBuilder<Incident>()
                        .WithTracking(true)
                        .WithPredicate(i => i.Id == id)
                        .WithInclude(i => i.KoiIncidents)
                        .WithInclude(i => i.PondIncidents)
                        .Build());

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

                bool wasResolved = incident.Status == IncidentStatus.Resolved;
                bool isNowResolved = dto.Status.HasValue && dto.Status.Value == IncidentStatus.Resolved;

                if (isNowResolved && !wasResolved)
                {
                    incident.ResolvedAt = DateTime.UtcNow;
                    incident.ResolvedByUserId = userId;             
                    if (incident.KoiIncidents != null && incident.KoiIncidents.Any())
                    {                    
                        var koiFishIds = incident.KoiIncidents
                            .Where(ki => !ki.RecoveredAt.HasValue)
                            .Select(ki => ki.KoiFishId)
                            .Distinct()
                            .ToList();

                        if (koiFishIds.Any())
                        {
                            var koiFishes = await _koiFishRepo.GetAllAsync(new QueryBuilder<KoiFish>()
                                .WithTracking(true)
                                .WithPredicate(k => koiFishIds.Contains(k.Id))
                                .Build());

                            var koiFishDict = koiFishes.ToDictionary(k => k.Id);

                            foreach (var koiIncident in incident.KoiIncidents.Where(ki => !ki.RecoveredAt.HasValue))
                            {
                                if (koiFishDict.TryGetValue(koiIncident.KoiFishId, out var koiFish) 
                                    && koiFish.HealthStatus != HealthStatus.Dead)
                                {
                                    koiFish.HealthStatus = HealthStatus.Healthy;
                                    koiFish.UpdatedAt = DateTime.UtcNow;
                                    await _koiFishRepo.UpdateAsync(koiFish);
                                }

                                koiIncident.RecoveredAt = DateTime.UtcNow;
                                await _koiIncidentRepo.UpdateAsync(koiIncident);
                            }
                        }
                    }

                    if (incident.PondIncidents != null && incident.PondIncidents.Any())
                    {                  
                        var pondIds = incident.PondIncidents.Select(pi => pi.PondId).Distinct().ToList();
                        var ponds = await _pondRepo.GetAllAsync(new QueryBuilder<Pond>()
                            .WithTracking(true)
                            .WithPredicate(p => pondIds.Contains(p.Id))
                            .Build());

                        foreach (var pond in ponds)
                        {
                            pond.PondStatus = PondStatus.Active;
                            await _pondRepo.UpdateAsync(pond);
                        }
                    }
                }

                if (dto.AffectedKoiFish != null && wasResolved)
                {
                    throw new InvalidOperationException("Không thể cập nhật danh sách cá bị ảnh hưởng khi sự cố đã được giải quyết.");
                }

                await _incidentRepo.UpdateAsync(incident);
           
                if (dto.AffectedKoiFish != null)
                {
                    var existingKoiIncidents = await _koiIncidentRepo.GetAllAsync(new QueryBuilder<KoiIncident>()
                        .WithTracking(true)
                        .WithPredicate(ki => ki.IncidentId == id)
                        .Build());

                    var existingKoiIncidentsList = existingKoiIncidents.ToList();
                    var existingKoiFishIds = existingKoiIncidentsList.Select(ki => ki.KoiFishId).ToHashSet();
                    var newKoiFishIds = dto.AffectedKoiFish.Select(d => d.KoiFishId).ToHashSet();
          
                    var allKoiFishIds = dto.AffectedKoiFish.Select(d => d.KoiFishId).Distinct().ToList();
                    var koiFishes = await _koiFishRepo.GetAllAsync(new QueryBuilder<KoiFish>()
                        .WithPredicate(k => allKoiFishIds.Contains(k.Id))
                        .Build());
                    var koiFishDict = koiFishes.ToDictionary(k => k.Id);

                    var toRemove = existingKoiIncidentsList.Where(ki => !newKoiFishIds.Contains(ki.KoiFishId)).ToList();
                    foreach (var removeItem in toRemove)
                    {
                        await _koiIncidentRepo.DeleteAsync(removeItem);
                    }

                    foreach (var koiDto in dto.AffectedKoiFish)
                    {
                        if (!koiFishDict.TryGetValue(koiDto.KoiFishId, out var koiFish))
                        {
                            throw new ArgumentException($"Không tìm thấy cá Koi với id {koiDto.KoiFishId}.");
                        }

                        var existingKoiIncident = existingKoiIncidentsList.FirstOrDefault(ki => ki.KoiFishId == koiDto.KoiFishId);

                        if (existingKoiIncident != null)
                        {
                            _mapper.Map(koiDto, existingKoiIncident);
                            await _koiIncidentRepo.UpdateAsync(existingKoiIncident);
                        }
                        else
                        {
                            var koiIncident = _mapper.Map<KoiIncident>(koiDto);
                            koiIncident.IncidentId = id;
                            await _koiIncidentRepo.CreateAsync(koiIncident);
                        }

                        if (!isNowResolved && koiFish.HealthStatus != HealthStatus.Dead)
                        {
                            koiFish.HealthStatus = koiDto.AffectedStatus;
                            koiFish.UpdatedAt = DateTime.UtcNow;
                            await _koiFishRepo.UpdateAsync(koiFish);
                        }
                    }
                }
          
                if (dto.AffectedPonds != null && wasResolved)
                {
                    throw new InvalidOperationException("Không thể cập nhật danh sách ao bị ảnh hưởng khi sự cố đã được giải quyết.");
                }
        
                if (dto.AffectedPonds != null)
                {
                    var existingPondIncidents = await _pondIncidentRepo.GetAllAsync(new QueryBuilder<PondIncident>()
                        .WithTracking(true)
                        .WithPredicate(pi => pi.IncidentId == id)
                        .Build());

                    var existingPondIncidentsList = existingPondIncidents.ToList();
                    var existingPondIds = existingPondIncidentsList.Select(pi => pi.PondId).ToHashSet();
                    var newPondIds = dto.AffectedPonds.Select(d => d.PondId).ToHashSet();

                    var allPondIds = dto.AffectedPonds.Select(d => d.PondId).Distinct().ToList();
                    var ponds = await _pondRepo.GetAllAsync(new QueryBuilder<Pond>()
                        .WithPredicate(p => allPondIds.Contains(p.Id))
                        .Build());
                    var pondDict = ponds.ToDictionary(p => p.Id);
                    var toRemove = existingPondIncidentsList.Where(pi => !newPondIds.Contains(pi.PondId)).ToList();
                    foreach (var removeItem in toRemove)
                    {
                        await _pondIncidentRepo.DeleteAsync(removeItem);
                    }

                    foreach (var pondDto in dto.AffectedPonds)
                    {
                        if (!pondDict.TryGetValue(pondDto.PondId, out var pond))
                        {
                            throw new ArgumentException($"Không tìm thấy ao với id {pondDto.PondId}.");
                        }

                        var existingPondIncident = existingPondIncidentsList.FirstOrDefault(pi => pi.PondId == pondDto.PondId);

                        if (existingPondIncident != null)
                        {
                            _mapper.Map(pondDto, existingPondIncident);
                            await _pondIncidentRepo.UpdateAsync(existingPondIncident);
                        }
                        else
                        {
                            var pondIncident = _mapper.Map<PondIncident>(pondDto);
                            pondIncident.IncidentId = id;
                            await _pondIncidentRepo.CreateAsync(pondIncident);
                        }

                        if (!isNowResolved)
                        {
                            pond.PondStatus = PondStatus.Maintenance;
                            await _pondRepo.UpdateAsync(pond);
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return await GetByIdAsync(id);
            }
            catch
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
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

        public async Task<IncidentResponseDTO> ChangeStatusAsync(int id, int userId, IncidentStatus status, string? resolutionNotes, List<string>? resolutionImages)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var incident = await _incidentRepo.GetSingleAsync(
                    BuildIncidentQueryBuilder()
                        .WithPredicate(i => i.Id == id)
                        .Build());

                if (incident == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy sự cố với id {id}.");
                }
           
                bool wasClosed = incident.Status == IncidentStatus.Resolved || incident.Status == IncidentStatus.Cancelled;
                bool isClosed = status == IncidentStatus.Resolved || status == IncidentStatus.Cancelled;

                incident.Status = status;
                incident.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(resolutionNotes))
                {
                    incident.ResolutionNotes = resolutionNotes;
                }

                if (resolutionImages != null && resolutionImages.Any())
                {
                    incident.ResolutionImages = resolutionImages;
                }

                if (isClosed && !wasClosed)
                {
                    incident.ResolvedAt = DateTime.UtcNow;
                    incident.ResolvedByUserId = userId;

                    if (incident.KoiIncidents != null && incident.KoiIncidents.Any())
                    {
                        var koiFishIds = incident.KoiIncidents
                            .Where(ki => !ki.RecoveredAt.HasValue)
                            .Select(ki => ki.KoiFishId)
                            .Distinct()
                            .ToList();

                        if (koiFishIds.Any())
                        {
                            var koiFishes = await _koiFishRepo.GetAllAsync(new QueryBuilder<KoiFish>()
                                .WithTracking(true)
                                .WithPredicate(k => koiFishIds.Contains(k.Id))
                                .Build());

                            var koiFishDict = koiFishes.ToDictionary(k => k.Id);

                            foreach (var koiIncident in incident.KoiIncidents.Where(ki => !ki.RecoveredAt.HasValue))
                            {
                                if (koiFishDict.TryGetValue(koiIncident.KoiFishId, out var koiFish) 
                                    && koiFish.HealthStatus != HealthStatus.Dead)
                                {
                                    koiFish.HealthStatus = HealthStatus.Healthy;
                                    koiFish.UpdatedAt = DateTime.UtcNow;
                                    await _koiFishRepo.UpdateAsync(koiFish);
                                }

                                koiIncident.RecoveredAt = DateTime.UtcNow;
                                await _koiIncidentRepo.UpdateAsync(koiIncident);
                            }
                        }
                    }

                    if (incident.PondIncidents != null && incident.PondIncidents.Any())
                    {
                        var pondIds = incident.PondIncidents.Select(pi => pi.PondId).Distinct().ToList();
                        var ponds = await _pondRepo.GetAllAsync(new QueryBuilder<Pond>()
                            .WithTracking(true)
                            .WithPredicate(p => pondIds.Contains(p.Id))
                            .Build());

                        foreach (var pond in ponds)
                        {
                            pond.PondStatus = PondStatus.Active;
                            await _pondRepo.UpdateAsync(pond);
                        }
                    }
                }

                await _incidentRepo.UpdateAsync(incident);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return await GetByIdAsync(id);
            }
            catch
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<KoiIncidentResponseDTO> AddKoiIncidentAsync(int incidentId, KoiIncidentRequestDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var incident = await _incidentRepo.GetByIdAsync(incidentId);
                if (incident == null)
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

                koiFish.HealthStatus = dto.AffectedStatus;
                koiFish.UpdatedAt = DateTime.UtcNow;
                await _koiFishRepo.UpdateAsync(koiFish);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<KoiIncidentResponseDTO>(koiIncident);
            }
            catch
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<PondIncidentResponseDTO> AddPondIncidentAsync(int incidentId, PondIncidentRequestDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var incident = await _incidentRepo.GetByIdAsync(incidentId);
                if (incident == null)
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
                pond.PondStatus = PondStatus.Maintenance;
                await _pondRepo.UpdateAsync(pond);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<PondIncidentResponseDTO>(pondIncident);
            }
            catch
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<KoiIncidentResponseDTO> UpdateKoiIncidentAsync(int koiIncidentId, UpdateKoiIncidentRequestDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var koiIncident = await _koiIncidentRepo.GetSingleAsync(new QueryBuilder<KoiIncident>()
                    .WithPredicate(ki => ki.Id == koiIncidentId)
                    .WithInclude(ki => ki.KoiFish)
                    .Build());

                if (koiIncident == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy KoiIncident với id {koiIncidentId}.");
                }
       
                if (dto.AffectedStatus.HasValue)
                {
                    var koiFish = await _koiFishRepo.GetByIdAsync(koiIncident.KoiFishId);
                    if (koiFish != null)
                    {
                        koiFish.HealthStatus = dto.AffectedStatus.Value;
                        koiFish.UpdatedAt = DateTime.UtcNow;
                        await _koiFishRepo.UpdateAsync(koiFish);
                    }
                }


                _mapper.Map(dto, koiIncident);
                await _koiIncidentRepo.UpdateAsync(koiIncident);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<KoiIncidentResponseDTO>(koiIncident);
            }
            catch
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<PondIncidentResponseDTO> UpdatePondIncidentAsync(int pondIncidentId, UpdatePondIncidentRequestDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var pondIncident = await _pondIncidentRepo.GetSingleAsync(new QueryBuilder<PondIncident>()
                    .WithPredicate(pi => pi.Id == pondIncidentId)
                    .WithInclude(pi => pi.Pond)
                    .Build());

                if (pondIncident == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy PondIncident với id {pondIncidentId}.");
                }


                _mapper.Map(dto, pondIncident);
                await _pondIncidentRepo.UpdateAsync(pondIncident);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<PondIncidentResponseDTO>(pondIncident);
            }
            catch
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<List<KoiIncidentResponseDTO>> GetKoiHealthHistoryAsync(int koiFishId)
        {

            var koiFish = await _koiFishRepo.GetByIdAsync(koiFishId);
            if (koiFish == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy cá Koi với id {koiFishId}.");
            }

            var koiIncidents = await _koiIncidentRepo.GetAllAsync(new QueryBuilder<KoiIncident>()
                .WithPredicate(ki => ki.KoiFishId == koiFishId)
                .WithInclude(ki => ki.KoiFish)
                .WithInclude(ki => ki.Incident)
                .WithInclude(ki => ki.Incident.IncidentType)
                .WithOrderBy(q => q.OrderByDescending(ki => ki.AffectedFrom))
                .Build());

            return _mapper.Map<List<KoiIncidentResponseDTO>>(koiIncidents);
        }

        private QueryBuilder<Incident> BuildIncidentQueryBuilder(bool tracked = false)
        {
            var queryBuilder = new QueryBuilder<Incident>()
                .WithTracking(tracked)
                .WithInclude(i => i.IncidentType)
                .WithInclude(i => i.ReportedBy)
                .WithInclude(i => i.KoiIncidents)
                .WithInclude(i => i.PondIncidents);
    
            return queryBuilder;
        }
    }
}

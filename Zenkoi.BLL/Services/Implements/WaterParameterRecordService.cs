using AutoMapper;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.WaterParameterRecordDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;
using System.Linq.Expressions;
using Zenkoi.DAL.Enums;
using Newtonsoft.Json;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using Zenkoi.BLL.WebSockets;
using Zenkoi.BLL.DTOs.WaterAlertDTOs;


namespace Zenkoi.BLL.Services.Implements
{
    public class WaterParameterRecordService : IWaterParameterRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<WaterParameterRecord> _recordRepo;
        private readonly IRepoBase<Pond> _pondRepo;
        private readonly IRepoBase<ApplicationUser> _userRepo;
        private readonly WebSocketConnectionManager _wsManager;
        private readonly ExpoPushNotificationService _pushService;
        private readonly IAccountService _accountService;


        public WaterParameterRecordService(IUnitOfWork unitOfWork, IMapper mapper, WebSocketConnectionManager wsManager, ExpoPushNotificationService pushService, IAccountService accountService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _recordRepo = _unitOfWork.GetRepo<WaterParameterRecord>();
            _pondRepo = _unitOfWork.GetRepo<Pond>();
            _userRepo = _unitOfWork.GetRepo<ApplicationUser>();
            _wsManager = wsManager;
            _pushService = pushService;
            _accountService = accountService;
        }

        public async Task<PaginatedList<WaterParameterRecordResponseDTO>> GetAllAsync(
            WaterParameterRecordFilterDTO? filter, int pageIndex = 1, int pageSize = 10)
        {
            filter ??= new WaterParameterRecordFilterDTO();

            var queryOptions = new QueryOptions<WaterParameterRecord>
            {
                IncludeProperties = new List<Expression<Func<WaterParameterRecord, object>>>
                {
                    r => r.Pond!,
                    r => r.RecordedBy!
                }
            };

            Expression<Func<WaterParameterRecord, bool>>? predicate = null;

            if (filter.PondId.HasValue)
                predicate = predicate.AndAlso(r => r.PondId == filter.PondId.Value);
            if (filter.FromDate.HasValue)
                predicate = predicate.AndAlso(r => r.RecordedAt >= filter.FromDate.Value);
            if (filter.ToDate.HasValue)
            {
                var toDate = filter.ToDate.Value.AddDays(1).AddSeconds(-1);
                predicate = predicate.AndAlso(r => r.RecordedAt <= toDate);
            }
            if (filter.RecordedByUserId.HasValue)
                predicate = predicate.AndAlso(r => r.RecordedByUserId == filter.RecordedByUserId.Value);
            if (!string.IsNullOrWhiteSpace(filter.NotesContains))
                predicate = predicate.AndAlso(r => r.Notes != null && r.Notes.Contains(filter.NotesContains));

            queryOptions.Predicate = predicate;

            var records = await _recordRepo.GetAllAsync(queryOptions);
            var mapped = _mapper.Map<List<WaterParameterRecordResponseDTO>>(records);

            foreach (var item in mapped)
            {
                item.PondName = (await _pondRepo.GetByIdAsync(item.PondId))?.PondName ?? "Không xác định";

                if (item.RecordedByUserId.HasValue)
                {
                    var user = await _userRepo.GetByIdAsync(item.RecordedByUserId.Value);
                    item.RecordedByUserName = user?.UserName ?? "Không xác định";
                }
            }

            var totalCount = mapped.Count;
            var pagedItems = mapped.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<WaterParameterRecordResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<WaterParameterRecordResponseDTO?> GetByIdAsync(int id)
        {
            var record = await _recordRepo.GetSingleAsync(new QueryOptions<WaterParameterRecord>
            {
                Predicate = r => r.Id == id,
                IncludeProperties = new List<Expression<Func<WaterParameterRecord, object>>>
                {
                    r => r.Pond!,
                    r => r.RecordedBy!
                }
            });

            if (record == null) return null;

            var result = _mapper.Map<WaterParameterRecordResponseDTO>(record);
            result.PondName = record.Pond?.PondName;
            result.RecordedByUserName = record.RecordedBy?.UserName;

            return result;
        }

   

        public async Task<WaterParameterRecordResponseDTO> CreateAsync(int userId, WaterParameterRecordRequestDTO dto)
        {
            var _alertRepo = _unitOfWork.GetRepo<WaterAlert>();
            var _thresholdRepo = _unitOfWork.GetRepo<WaterParameterThreshold>();

            var pond = await _pondRepo.GetSingleAsync(new QueryOptions<Pond>
            {
                Predicate = p => p.Id == dto.PondId
            });

            if (pond == null)
                throw new KeyNotFoundException($"Không tìm thấy ao với Id = {dto.PondId}");

            var thresholds = await _thresholdRepo.GetAllAsync(new QueryOptions<WaterParameterThreshold>
            {
                Predicate = t => t.PondTypeId == pond.PondTypeId
            });

            // Lưu record mới
            var entity = _mapper.Map<WaterParameterRecord>(dto);
            entity.RecordedAt = DateTime.UtcNow;
            entity.RecordedByUserId = userId;

            await _recordRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            await CheckAndCreateAlertsAsync(entity, thresholds, _alertRepo);

            var result = _mapper.Map<WaterParameterRecordResponseDTO>(entity);
            result.PondName = pond.PondName;
            if (entity.RecordedByUserId.HasValue)
                result.RecordedByUserName = (await _userRepo.GetByIdAsync(entity.RecordedByUserId.Value))?.UserName;

            return result;
        }


        public async Task<WaterParameterRecordResponseDTO?> UpdateAsync(int id, WaterParameterRecordRequestDTO dto)
        {
            var entity = await _recordRepo.GetByIdAsync(id);
            if (entity == null) return null;

            // Kiểm tra Pond tồn tại
            if (dto.PondId != entity.PondId)
            {
                var pondExists = await _pondRepo.CheckExistAsync(dto.PondId);
                if (!pondExists)
                    throw new KeyNotFoundException($"Không tìm thấy ao với Id = {dto.PondId}");
            }

            _mapper.Map(dto, entity);
            entity.RecordedAt = DateTime.UtcNow;

            await _recordRepo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Kiểm tra vượt ngưỡng sau khi cập nhật
            var _alertRepo = _unitOfWork.GetRepo<WaterAlert>();
            var _thresholdRepo = _unitOfWork.GetRepo<WaterParameterThreshold>();

            var pond = await _pondRepo.GetByIdAsync(entity.PondId);
            var thresholds = await _thresholdRepo.GetAllAsync(new QueryOptions<WaterParameterThreshold>
            {
                Predicate = t => t.PondTypeId == pond!.PondTypeId
            });

            await CheckAndCreateAlertsAsync(entity, thresholds, _alertRepo);

            var result = _mapper.Map<WaterParameterRecordResponseDTO>(entity);
            result.PondName = pond?.PondName;
            result.RecordedByUserName = entity.RecordedByUserId.HasValue
                ? (await _userRepo.GetByIdAsync(entity.RecordedByUserId.Value))?.UserName
                : null;

            return result;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var _waterAlertRepo =  _unitOfWork.GetRepo<WaterAlert>();
            var entity = await _recordRepo.GetByIdAsync(id);
            if (entity == null) return false;

          
            var alerts = await _waterAlertRepo
                .GetAllAsync(new QueryOptions<WaterAlert>
                {
                    Predicate = P => P.WaterParameterRecord.Id == id
                });

            if (alerts.Any())
                await _waterAlertRepo.DeleteAllAsync(alerts.ToList());

            await _recordRepo.DeleteAsync(entity);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }


        private async Task CheckAndCreateAlertsAsync(WaterParameterRecord entity, IEnumerable<WaterParameterThreshold> thresholds, IRepoBase<WaterAlert> _alertRepo)
        {
            var parameterValues = new Dictionary<WaterParameterType, double?>
            {
                { WaterParameterType.PHLevel, entity.PHLevel },
                { WaterParameterType.TemperatureCelsius, entity.TemperatureCelsius },
                { WaterParameterType.OxygenLevel, entity.OxygenLevel },
                { WaterParameterType.AmmoniaLevel, entity.AmmoniaLevel },
                { WaterParameterType.NitriteLevel, entity.NitriteLevel },
                { WaterParameterType.NitrateLevel, entity.NitrateLevel },
                { WaterParameterType.CarbonHardness, entity.CarbonHardness },
                { WaterParameterType.WaterLevelMeters, entity.WaterLevelMeters }
            };

            foreach (var kv in parameterValues)
            {
                var parameter = kv.Key;
                var value = kv.Value;
                if (value == null) continue;

                var threshold = thresholds.FirstOrDefault(t => t.ParameterName == parameter);
                if (threshold == null) continue;

                AlertType? alertType = null;
                string message = null;

                string ParameterName = TranslateParameterName(parameter);
                if (value < threshold.MinValue)
                {
                    alertType = AlertType.Low;
                    message = $"Giá trị {ParameterName} = {value} {threshold.Unit} thấp hơn ngưỡng tối thiểu {threshold.MinValue}.";
                }
                else if (value > threshold.MaxValue)
                {
                    alertType = AlertType.High;
                    message = $"Giá trị {ParameterName} = {value} {threshold.Unit} vượt ngưỡng tối đa {threshold.MaxValue}.";
                }

                if (alertType != null)
                {
                    var severity = GetSeverityLevel(threshold, value.Value);

                    var alert = new WaterAlert
                    {
                        PondId = entity.PondId,
                        ParameterName = parameter,
                        MeasuredValue = value.Value,
                        AlertType = alertType.Value,
                        Severity = severity,
                        Message = message,
                        CreatedAt = DateTime.UtcNow,
                        IsResolved = false,
                        WaterParameterRecord = entity
                    };

                    await _alertRepo.CreateAsync(alert);
                    

                    var dto = new WaterAlertWebSocketResponseDTO
                    {
                        PondId = alert.PondId,
                        PondName = alert.Pond.PondName,
                        ParameterName = ParameterName,
                        MeasuredValue = alert.MeasuredValue,
                        AlertType = alert.AlertType.ToString(),
                        Severity = alert.Severity.ToString(),
                        Message = alert.Message,
                        CreatedAt = alert.CreatedAt,
                        IsResolved = alert.IsResolved,
                    };
                    var payload = JsonConvert.SerializeObject(dto);
                    await _wsManager.BroadcastAsync(payload);

                    var tokens = (await _accountService.GetStaffManagerTokensAsync())
                                    .Where(t => !string.IsNullOrWhiteSpace(t));

                    await _pushService.SendPushNotificationToMultipleAsync(
                        tokens,
                        $"Alert: {ParameterName} vượt ngưỡng",
                        message,
                        new { PondId = entity.PondId, Parameter = parameter, Value = value }
                    );
                }
            }

           
            await _unitOfWork.SaveChangesAsync();
        }

        private SeverityLevel GetSeverityLevel(WaterParameterThreshold threshold, double value)
        {
            var range = threshold.MaxValue - threshold.MinValue;
            if (range <= 0)
                return SeverityLevel.Medium;

            double delta = 0;
            if (value < threshold.MinValue)
                delta = threshold.MinValue - value;
            else if (value > threshold.MaxValue)
                delta = value - threshold.MaxValue;

            double ratio = delta / range;

            if (ratio < 0.1)
                return SeverityLevel.Low;
            else if (ratio < 0.25)
                return SeverityLevel.Medium;
            else if (ratio < 0.5)
                return SeverityLevel.High;
            else
                return SeverityLevel.Urgent;
        }
        private string TranslateParameterName(WaterParameterType parameter)
        {
            return parameter switch
            {
                WaterParameterType.PHLevel => "Độ pH",
                WaterParameterType.TemperatureCelsius => "Nhiệt độ",
                WaterParameterType.OxygenLevel => "Hàm lượng Oxy hòa tan",
                WaterParameterType.AmmoniaLevel => "Nồng độ Amoniac",
                WaterParameterType.NitriteLevel => "Nồng độ Nitrit",
                WaterParameterType.NitrateLevel => "Nồng độ Nitrat",
                WaterParameterType.CarbonHardness => "Độ cứng cacbonat",
                WaterParameterType.WaterLevelMeters => "Mực nước",
                _ => parameter.ToString()
            };
        }


    }
}

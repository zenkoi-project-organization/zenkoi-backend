using AutoMapper;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using Zenkoi.BLL.DTOs.ApplicationUserDTOs;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.BreedingDTOs;
using Zenkoi.BLL.DTOs.ClassificationRecordDTOs;
using Zenkoi.BLL.DTOs.ClassificationStageDTOs;
using Zenkoi.BLL.DTOs.EggBatchDTOs;
using Zenkoi.BLL.DTOs.FryFishDTOs;
using Zenkoi.BLL.DTOs.FrySurvivalRecordDTOs;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.BLL.DTOs.PondDTOs;
using Zenkoi.BLL.DTOs.PondTypeDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.BLL.DTOs.PacketFishDTOs;
using Zenkoi.BLL.DTOs.CustomerDTOs;
using Zenkoi.BLL.DTOs.CartDTOs;
using Zenkoi.BLL.DTOs.TaskTemplateDTOs;
using Zenkoi.BLL.DTOs.WorkScheduleDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.BLL.DTOs.PondPacketFishDTOs;
using Zenkoi.BLL.DTOs.ShippingBoxDTOs;
using Zenkoi.BLL.DTOs.ShippingDistanceDTOs;
using Zenkoi.BLL.DTOs.WaterParameterThresholdDTOs;
using Zenkoi.BLL.DTOs.WaterParameterRecordDTOs;
using Zenkoi.BLL.DTOs.CustomerAddressDTOs;
using Zenkoi.BLL.DTOs.WeeklyScheduleTemplateDTOs;
using Zenkoi.BLL.DTOs.IncidentDTOs;
using Zenkoi.BLL.DTOs.IncidentTypeDTOs;
using Zenkoi.BLL.DTOs.PaymentTransactionDTOs;

namespace Zenkoi.BLL.Helpers.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            var dalAssembly = Assembly.Load("Zenkoi.DAL");
            var bllAssembly = Assembly.Load("Zenkoi.BLL");


            var entityTypes = dalAssembly.GetTypes().Where(t => t.IsClass && t.Namespace == "Zenkoi.DAL.Entities");


            foreach (var entityType in entityTypes)
            {
                var dtoTypes = bllAssembly.GetTypes()
                    .Where(t => t.IsClass && t.Namespace == $"Zenkoi.BLL.DTOs.{entityType.Name}DTOs" && t.Name.StartsWith(entityType.Name));

                foreach (var dtoType in dtoTypes)
                {
                    CreateMap(entityType, dtoType).ReverseMap();
                }
            }
            CreateMap<ApplicationUser, ApplicationUserResponseDTO>();
            CreateMap<Area, AreaResponseDTO>().ReverseMap();
            CreateMap<AreaRequestDTO, Area>();
            CreateMap<PondType, PondTypeResponseDTO>().ReverseMap();
            CreateMap<PondTypeRequestDTO, PondType>();
            CreateMap<Pond, PondResponseDTO>()
                .ForMember(dest => dest.PondTypeName, opt => opt.MapFrom(src => src.PondType.TypeName))
                .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.AreaName))
                .ReverseMap();
            CreateMap<PondRequestDTO, Pond>();
            CreateMap<Variety, VarietyResponseDTO>().ReverseMap();
            CreateMap<VarietyRequestDTO, Variety>();
            CreateMap<double, FishSize>().ConvertUsing<FishSizeConverter>();
            CreateMap<KoiFish, KoiFishResponseDTO>().ReverseMap();
            CreateMap<KoiFishRequestDTO, KoiFish>()
            .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size));
            CreateMap<KoiFish, KoiGrandParentDTO>()
           .ForMember(dest => dest.VarietyName, opt => opt.MapFrom(src => src.Variety.VarietyName));
            CreateMap<Pond, PondBasicDTO>();
            CreateMap<Variety, VarietyBasicDTO>();
            CreateMap<BreedingProcess, BreedingProcessBasicDTO>();

            CreateMap<BreedingProcess, BreedingProcessResponseDTO>()
            .ForMember(dest => dest.MaleKoiRFID,
                opt => opt.MapFrom(src => src.MaleKoi != null ? src.MaleKoi.RFID : null))
            .ForMember(dest => dest.FemaleKoiRFID,
                opt => opt.MapFrom(src => src.FemaleKoi != null ? src.FemaleKoi.RFID : null))
            .ForMember(dest => dest.PondName,
                opt => opt.MapFrom(src => src.Pond != null ? src.Pond.PondName : null))
              .ForMember(dest => dest.HatchedTime,
               opt => opt.MapFrom(src => src.Batch != null ? src.Batch.HatchingTime : null))
            .ForMember(dest => dest.MaleKoiVariety, otp => otp.MapFrom(src => src.MaleKoi.Variety.VarietyName))
            .ForMember(dest => dest.FemaleKoiVariety, otp => otp.MapFrom(src => src.FemaleKoi.Variety.VarietyName))
            .ForMember(dest => dest.KoiFishes,
                opt => opt.MapFrom(src => src.KoiFishes != null
            ? src.KoiFishes.Select(k => new KoiFishResponseDTO
            {
                Id = k.Id,
                RFID = k.RFID,
                Gender = k.Gender,
            }).ToList()
            : new List<KoiFishResponseDTO>()));

            CreateMap<BreedingProcess, BreedingResponseDTO>()
            .ForMember(dest => dest.MaleKoiRFID,
                opt => opt.MapFrom(src => src.MaleKoi != null ? src.MaleKoi.RFID : null))
            .ForMember(dest => dest.FemaleKoiRFID,
                opt => opt.MapFrom(src => src.FemaleKoi != null ? src.FemaleKoi.RFID : null))
            .ForMember(dest => dest.PondName,
                opt => opt.MapFrom(src => src.Pond != null ? src.Pond.PondName : null))
            .ForMember(dest => dest.HatchedTime,
               opt => opt.MapFrom(src => src.Batch != null ? src.Batch.HatchingTime : null))
            .ForMember(dest => dest.MaleKoiVariety, otp => otp.MapFrom(src => src.MaleKoi.Variety.VarietyName))
            .ForMember(dest => dest.FemaleKoiVariety, otp => otp.MapFrom(src => src.FemaleKoi.Variety.VarietyName))
            .ForMember(dest => dest.KoiFishes,
                opt => opt.MapFrom(src => src.KoiFishes != null
            ? src.KoiFishes.Select(k => new KoiFishResponseDTO
            {
                Id = k.Id,
                RFID = k.RFID,
                Gender = k.Gender,
            }).ToList()
            : new List<KoiFishResponseDTO>()));



            CreateMap<BreedingProcessRequestDTO, BreedingProcess>();
            CreateMap<EggBatch, EggBatchResponseDTO>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<EggBatchRequestDTO, EggBatch>();
            CreateMap<EggBatchUpdateRequestDTO, EggBatch>();
            CreateMap<FryFish, FryFishResponseDTO>().ReverseMap();
            CreateMap<FryFishRequestDTO, FryFish>();
            CreateMap<FrySurvivalRecord, FrySurvivalRecordResponseDTO>().ReverseMap();
            CreateMap<FrySurvivalRecordRequestDTO, FrySurvivalRecord>();
            CreateMap<ClassificationStage, ClassificationStageResponseDTO>();          
            CreateMap<ClassificationStageCreateRequestDTO, ClassificationStage>();
            CreateMap<ClassificationStageUpdateRequestDTO, ClassificationStage>();
            CreateMap<ClassificationRecord, ClassificationRecordResponseDTO>().ReverseMap();
            CreateMap<ClassificationRecordRequestDTO, ClassificationRecord>();
            CreateMap<ClassificationRecordUpdateRequestDTO, ClassificationRecord>();
            CreateMap<PondPacketFishRequestDTO, PondPacketFish>();
            CreateMap<PondPacketFish, PondPacketFishResponseDTO>();
            CreateMap<WaterParameterThreshold, WaterParameterThresholdResponseDTO>()
           .ForMember(dest => dest.PondTypeName,
                      opt => opt.MapFrom(src => src.PondType != null ? src.PondType.TypeName : null));

            CreateMap<WaterParameterThresholdRequestDTO, WaterParameterThreshold>();
            CreateMap<WaterParameterRecord, WaterParameterRecordResponseDTO>()
            .ForMember(dest => dest.PondName, opt => opt.MapFrom(src => src.Pond.PondName))
            .ForMember(dest => dest.RecordedByUserName, opt => opt.MapFrom(src => src.RecordedBy != null ? src.RecordedBy.UserName : null));
            CreateMap<WaterParameterRecordRequestDTO, WaterParameterRecord>();
            CreateMap<WaterRecordDTO, WaterParameterRecordRequestDTO>();
            CreateMap<WaterParameterRecordRequestDTO, WaterRecordDTO>();
            CreateMap<WaterParameterRecord, WaterRecordDTO>().ReverseMap();


            // Order mappings
            CreateMap<OrderDetail, OrderDetailResponseDTO>()
              .ForMember(dest => dest.KoiFish, opt => opt.MapFrom(src => src.KoiFish))
              .ForMember(dest => dest.PacketFish, opt => opt.MapFrom(src => src.PacketFish));

            CreateMap<OrderDetail, OrderDetailResponseDTO>()
            .ForMember(dest => dest.KoiFish, opt => opt.MapFrom(src => src.KoiFish))
            .ForMember(dest => dest.PacketFish, opt => opt.MapFrom(src => src.PacketFish));

            CreateMap<Order, OrderResponseDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                    src.Customer != null && src.Customer.ApplicationUser != null
                        ? src.Customer.ApplicationUser.FullName
                        : string.Empty))
                .ForMember(dest => dest.Promotion, opt => opt.MapFrom(src => src.Promotion))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

            // PacketFish mappings
            CreateMap<PacketFish, PacketFishResponseDTO>();
            CreateMap<PacketFishRequestDTO, PacketFish>();
            CreateMap<PacketFishUpdateDTO, PacketFish>();
            CreateMap<VarietyPacketFish, VarietyPacketFishResponseDTO>()
                .ForMember(dest => dest.VarietyName, opt => opt.MapFrom(src => src.Variety != null ? src.Variety.VarietyName : string.Empty));
   

            // Customer mappings
            CreateMap<Customer, CustomerResponseDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.ApplicationUser != null ? src.ApplicationUser.UserName : string.Empty))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.ApplicationUser != null ? src.ApplicationUser.FullName : string.Empty))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ApplicationUser != null ? src.ApplicationUser.Email : string.Empty))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.ApplicationUser != null ? src.ApplicationUser.PhoneNumber : null))
                .ForMember(dest => dest.RecentOrders, opt => opt.MapFrom(src => src.Orders != null
                    ? src.Orders.OrderByDescending(o => o.CreatedAt).Take(5).Select(o => new CustomerOrderSummaryDTO
                    {
                        Id = o.Id,
                        OrderNumber = o.OrderNumber,
                        CreatedAt = o.CreatedAt,
                        Status = o.Status,
                        TotalAmount = o.TotalAmount
                    }).ToList()
                    : new List<CustomerOrderSummaryDTO>()));
            CreateMap<CustomerRequestDTO, Customer>();
            CreateMap<CustomerUpdateDTO, Customer>();

          
            CreateMap<CartItem, CartItemResponseDTO>()
                .ForMember(dest => dest.KoiFish, opt => opt.MapFrom(src => src.KoiFish))
                .ForMember(dest => dest.PacketFish, opt => opt.MapFrom(src => src.PacketFish))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<Cart, CartResponseDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                    src.Customer != null && src.Customer.ApplicationUser != null
                        ? src.Customer.ApplicationUser.FullName
                        : string.Empty))
                .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.CartItems));

            CreateMap<TaskTemplate, TaskTemplateResponseDTO>().ReverseMap();
            CreateMap<TaskTemplateRequestDTO, TaskTemplate>();

            CreateMap<WorkSchedule, WorkScheduleResponseDTO>()
                .ForMember(dest => dest.TaskTemplateName, opt => opt.MapFrom(src => src.TaskTemplate != null ? src.TaskTemplate.TaskName : string.Empty))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.FullName : string.Empty))
                .ForMember(dest => dest.TaskTemplate, opt => opt.MapFrom(src => src.TaskTemplate))
                .ForMember(dest => dest.StaffAssignments, opt => opt.MapFrom(src => src.StaffAssignments))
                .ForMember(dest => dest.PondAssignments, opt => opt.MapFrom(src => src.PondAssignments));
            CreateMap<WorkScheduleRequestDTO, WorkSchedule>();

            CreateMap<StaffAssignment, StaffAssignmentResponseDTO>()
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff != null ? src.Staff.FullName : string.Empty))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Staff != null ? src.Staff.Role : Role.Customer));

            CreateMap<PondAssignment, PondAssignmentResponseDTO>()
                .ForMember(dest => dest.PondName, opt => opt.MapFrom(src => src.Pond != null ? src.Pond.PondName : string.Empty));

            CreateMap<ShippingBox, ShippingBoxResponseDTO>().ReverseMap();
            CreateMap<ShippingBoxRequestDTO, ShippingBox>();
            CreateMap<ShippingBoxRule, ShippingBoxRuleResponseDTO>().ReverseMap();
            CreateMap<ShippingBoxRuleRequestDTO, ShippingBoxRule>();
            CreateMap<ShippingBoxRuleUpdateDTO, ShippingBoxRule>();

            CreateMap<ShippingDistance, ShippingDistanceResponseDTO>().ReverseMap();
            CreateMap<ShippingDistanceRequestDTO, ShippingDistance>();

            CreateMap<WeeklyScheduleTemplate, WeeklyScheduleTemplateResponseDTO>().ReverseMap();
            CreateMap<WeeklyScheduleTemplateRequestDTO, WeeklyScheduleTemplate>();
            CreateMap<WeeklyScheduleTemplateItem, WeeklyScheduleTemplateItemResponseDTO>().ReverseMap();
            CreateMap<WeeklyScheduleTemplateItemDTO, WeeklyScheduleTemplateItem>();

            CreateMap<Incident, IncidentResponseDTO>()
                .ForMember(dest => dest.IncidentTypeName, opt => opt.MapFrom(src => src.IncidentType != null ? src.IncidentType.Name : string.Empty))
                .ForMember(dest => dest.ReportedByUserName, opt => opt.MapFrom(src => src.ReportedBy != null ? src.ReportedBy.FullName : string.Empty))
                .ForMember(dest => dest.ResolvedByUserName, opt => opt.MapFrom(src => src.ResolvedBy != null ? src.ResolvedBy.FullName : null))
                .ForMember(dest => dest.KoiIncidents, opt => opt.MapFrom(src => src.KoiIncidents ?? new List<KoiIncident>()))
                .ForMember(dest => dest.PondIncidents, opt => opt.MapFrom(src => src.PondIncidents ?? new List<PondIncident>()));
            CreateMap<IncidentRequestDTO, Incident>();
            CreateMap<CreateIncidentWithDetailsDTO, Incident>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => IncidentStatus.Reported))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<IncidentUpdateRequestDTO, Incident>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<KoiIncident, KoiIncidentSimpleDTO>()
                .ForMember(dest => dest.KoiFishRFID, opt => opt.MapFrom(src => src.KoiFish != null ? src.KoiFish.RFID : string.Empty))
                .ForMember(dest => dest.SpecificSymptoms, opt => opt.MapFrom(src => src.SpecificSymptoms ?? string.Empty));
            CreateMap<KoiIncident, KoiIncidentResponseDTO>()
                .ForMember(dest => dest.KoiFishRFID, opt => opt.MapFrom(src => src.KoiFish != null ? src.KoiFish.RFID : null))
                .ForMember(dest => dest.SpecificSymptoms, opt => opt.MapFrom(src => src.SpecificSymptoms))
                .ForMember(dest => dest.TreatmentNotes, opt => opt.MapFrom(src => src.TreatmentNotes))
                .ForMember(dest => dest.Incident, opt => opt.MapFrom(src => src.Incident));
            CreateMap<KoiIncidentRequestDTO, KoiIncident>();
            CreateMap<UpdateKoiIncidentRequestDTO, KoiIncident>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Incident, IncidentSimpleDTO>()
                .ForMember(dest => dest.IncidentTypeName, opt => opt.MapFrom(src => src.IncidentType != null ? src.IncidentType.Name : string.Empty));
            CreateMap<PondIncident, PondIncidentSimpleDTO>()
                .ForMember(dest => dest.PondName, opt => opt.MapFrom(src => src.Pond != null ? src.Pond.PondName : string.Empty))
                .ForMember(dest => dest.EnvironmentalChanges, opt => opt.MapFrom(src => src.EnvironmentalChanges ?? string.Empty));
            CreateMap<PondIncident, PondIncidentResponseDTO>()
                .ForMember(dest => dest.PondName, opt => opt.MapFrom(src => src.Pond != null ? src.Pond.PondName : null))
                .ForMember(dest => dest.EnvironmentalChanges, opt => opt.MapFrom(src => src.EnvironmentalChanges))
                .ForMember(dest => dest.CorrectiveActions, opt => opt.MapFrom(src => src.CorrectiveActions))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));
            CreateMap<PondIncidentRequestDTO, PondIncident>();
            CreateMap<UpdatePondIncidentRequestDTO, PondIncident>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<IncidentType, IncidentTypeResponseDTO>().ReverseMap();
            CreateMap<IncidentTypeRequestDTO, IncidentType>();
            CreateMap<IncidentTypeUpdateRequestDTO, IncidentType>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<PaymentTransaction, PaymentTransactionResponseDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                    src.User != null ? (src.User.FullName ?? src.User.UserName) : string.Empty))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.Email : string.Empty));
        }
    }
}

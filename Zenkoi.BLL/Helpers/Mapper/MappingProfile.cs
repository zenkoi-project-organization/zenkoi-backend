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
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;

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
			CreateMap<PondType,PondTypeResponseDTO>().ReverseMap();
			CreateMap<PondTypeRequestDTO, PondType>();
			CreateMap<Pond, PondResponseDTO>()
                .ForMember(dest => dest.PondTypeName, opt => opt.MapFrom(src => src.PondType.TypeName))
                .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.AreaName))
                .ReverseMap();
			CreateMap<PondRequestDTO, Pond>();
			CreateMap<Variety , VarietyResponseDTO>().ReverseMap();
			CreateMap<VarietyRequestDTO, Variety>();
            CreateMap<double, FishSize>().ConvertUsing<FishSizeConverter>();
            CreateMap<KoiFish,KoiFishResponseDTO>().ReverseMap();
            CreateMap<KoiFishRequestDTO, KoiFish>()
            .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size));
            CreateMap<Pond, PondBasicDTO>();
            CreateMap<Variety, VarietyBasicDTO>();
            CreateMap<BreedingProcess, BreedingProcessResponseDTO>()
            .ForMember(dest => dest.MaleKoiRFID,
                opt => opt.MapFrom(src => src.MaleKoi != null ? src.MaleKoi.RFID : null))
            .ForMember(dest => dest.FemaleKoiRFID,
                opt => opt.MapFrom(src => src.FemaleKoi != null ? src.FemaleKoi.RFID : null))
            .ForMember(dest => dest.PondName,
                opt => opt.MapFrom(src => src.Pond != null ? src.Pond.PondName : null))
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
            .ForMember(dest => dest.PondName,
                opt => opt.MapFrom(src => src.Pond.PondName))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<EggBatchRequestDTO, EggBatch>();
            CreateMap<EggBatchUpdateRequestDTO, EggBatch>();
            CreateMap<FryFish, FryFishResponseDTO>().ReverseMap();
            CreateMap<FryFishRequestDTO, FryFish>();
            CreateMap<FrySurvivalRecord, FrySurvivalRecordResponseDTO>().ReverseMap();
            CreateMap<FrySurvivalRecordRequestDTO,FrySurvivalRecord>();
            CreateMap<ClassificationStage, ClassificationStageResponseDTO>()
            .ForMember(dest => dest.PondName, opt => opt.MapFrom(src => src.Pond.PondName)).ReverseMap();
            CreateMap<ClassificationStageCreateRequestDTO, ClassificationStage>();
            CreateMap<ClassificationStageUpdateRequestDTO, ClassificationStage>();
            CreateMap<ClassificationRecord, ClassificationRecordResponseDTO>().ReverseMap();
            CreateMap<ClassificationRecordRequestDTO, ClassificationRecord>();
            CreateMap<ClassificationRecordUpdateRequestDTO, ClassificationRecord>();
        }
	}
}

using AutoMapper;
using System.Globalization;
using System.Reflection;
using Zenkoi.BLL.DTOs.ApplicationUserDTOs;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.BLL.DTOs.PondDTOs;
using Zenkoi.BLL.DTOs.PondTypeDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.DAL.Entities;

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
			CreateMap<Pond, PondResponseDTO>().ReverseMap();
			CreateMap<PondRequestDTO, Pond>();
			CreateMap<Variety , VarietyResponseDTO>().ReverseMap();
			CreateMap<VarietyRequestDTO, Variety>();
			CreateMap<KoiFish,KoiFishResponseDTO>().ReverseMap();
			CreateMap<KoiFishRequestDTO, KoiFish>();
            CreateMap<Pond, PondBasicDTO>();
            CreateMap<Variety, VarietyBasicDTO>();
        }
	}
}

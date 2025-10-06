using AutoMapper;
using System.Globalization;
using System.Reflection;
using Zenkoi.BLL.DTOs.ApplicationUserDTOs;
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
        }
	}
}

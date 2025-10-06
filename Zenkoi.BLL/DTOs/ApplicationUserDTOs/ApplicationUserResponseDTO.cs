

using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.ApplicationUserDTOs
{
	public class ApplicationUserResponseDTO
	{
		public int Id { get; set; }
		public string FullName { get; set; }
		public Role Role { get; set; }
		public bool IsDeleted { get; set; }
		public string? Email { get; set; }

	}
}

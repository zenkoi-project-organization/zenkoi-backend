using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
	public class UserDetail
	{
		public int Id { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public Gender Gender { get; set; }
		public string? AvatarURL { get; set; }
		public string? Address { get; set; }
		public int ApplicationUserId { get; set; }

		public ApplicationUser User { get; set; }

	}
}

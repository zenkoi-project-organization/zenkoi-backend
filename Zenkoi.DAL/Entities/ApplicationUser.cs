using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
	public class ApplicationUser : IdentityUser<int>
	{
		public string FullName { get; set; }
		public Role Role { get; set; }
		public bool IsDeleted { get; set; }
		public UserDetail UserDetail { get; set; }
        public string? ExpoPushToken { get; set; }
        public bool IsBlocked { get; set; }
    }
}

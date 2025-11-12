using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.ApplicationUserDTOs
{
    public class UpdateUserProfileDTO
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        // UserDetail fields
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }

        [MaxLength(500)]
        public string? AvatarURL { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }
    }
}

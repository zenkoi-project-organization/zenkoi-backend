using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.AccountDTOs
{
    public class StaffAccountResponseDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public Role Role { get; set; }
        public string TempPassword { get; set; }
    }
}

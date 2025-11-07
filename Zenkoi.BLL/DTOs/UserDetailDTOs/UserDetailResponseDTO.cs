using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Entities;

namespace Zenkoi.BLL.DTOs.UserDetailDTOs
{
    public class UserDetailResponseDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string phoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string AvatarURL { get; set; }
        public string Address { get; set; }

        public string Role  {  get; set; }
      //  public ApplicationUser User { get; set; }
    }
}

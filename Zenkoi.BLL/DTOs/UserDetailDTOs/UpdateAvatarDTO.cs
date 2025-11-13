using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.UserDetailDTOs
{
	public class UpdateAvatarDTO
	{
		[Required(ErrorMessage = "Avatar URL không được để trống.")]
		[Url(ErrorMessage = "Avatar URL không hợp lệ.")]
		public string AvatarURL { get; set; }
	}
}

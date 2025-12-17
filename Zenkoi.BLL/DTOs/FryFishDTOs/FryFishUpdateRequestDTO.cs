using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.FryFishDTOs
{
    public class FryFishUpdateRequestDTO
    {
        [Required(ErrorMessage = "vui lòng chọn hồ")]
        public int PondId { get; set; }
    }
}

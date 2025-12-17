using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.EggBatchDTOs
{
    public class EggBatchRequestDTO
    {
        [Required(ErrorMessage = "BreedingProcessId không được để trống")]
        public int BreedingProcessId { get; set; }

        [Required(ErrorMessage = "vui lòng chọn hồ")]
        public int PondId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải ≥ 0")]
        public int? Quantity { get; set; }
    }
}

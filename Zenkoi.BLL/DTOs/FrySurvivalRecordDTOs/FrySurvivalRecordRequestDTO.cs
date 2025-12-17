using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.FrySurvivalRecordDTOs
{
    public class FrySurvivalRecordRequestDTO
    {
        [Required(ErrorMessage = "FryFishId không được để trống")]
        public int FryFishId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng cá sống phải ≥ 0")]
        public int? CountAlive { get; set; }

        [MaxLength(500, ErrorMessage = "Note tối đa 500 ký tự")]
        public string? Note { get; set; }

        public bool Success { get; set; }
    }
}

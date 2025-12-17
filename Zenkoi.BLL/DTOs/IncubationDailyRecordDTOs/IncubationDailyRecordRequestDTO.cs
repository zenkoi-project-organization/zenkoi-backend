using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.IncubationDailyRecordDTOs
{
     public class IncubationDailyRecordRequestDTO
    {
        [Required(ErrorMessage = "EggBatchId không được để trống")]
        public int EggBatchId { get; set; }

        [Required(ErrorMessage = "Số trứng khỏe không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "sô trứng khỏe phải ≥ 0")]
        public int? HealthyEggs { get; set; }
        [Required(ErrorMessage = "số trứng nở không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "số trứng nở phải ≥ 0")]
        public int? HatchedEggs { get; set; }

        public bool Success { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.EggBatchDTOs
{
    public class EggBatchResponseDTO
    {
        public int Id { get; set; }
        public int BreedingProcessId { get; set; }
        public int PondId { get; set; }
        public string? PondName { get; set; } 

        public int? Quantity { get; set; }
        public double? FertilizationRate { get; set; }
        public string Status { get; set; }
        public DateTime? HatchingTime { get; set; }
        public DateTime? SpawnDate { get; set; }

        // Có thể thêm danh sách bản ghi theo dõi nếu cần
       // public List<IncubationDailyRecordResponse>? IncubationDailyRecords { get; set; }
    }
}

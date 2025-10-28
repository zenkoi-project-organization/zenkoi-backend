using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.ClassificationRecordDTOs;

namespace Zenkoi.BLL.DTOs.ClassificationStageDTOs
{
    public class ClassificationStageResponseDTO
    {
            public int Id { get; set; }
            public int BreedingProcessId { get; set; }
            public int? TotalCount { get; set; }
            public string Status { get; set; }
            public int? HighQualifiedCount { get; set; }
            public int? ShowQualifiedCount { get; set; }
            public int? PondQualifiedCount { get; set; }
            public string Notes { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }

            // Nếu cần trả về danh sách các record chi tiết
            public List<ClassificationRecordResponseDTO> ClassificationRecords { get; set; }
        }
    }
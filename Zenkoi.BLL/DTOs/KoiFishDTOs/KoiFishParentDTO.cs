using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.KoiFishDTOs
{
    public class KoiFishParentResponseDTO
    {
     public int KoiFishId { get; set; }
    public int ParticipationCount { get; set; }
    public int FailCount { get; set; }
    public double? FertilizationRate { get; set; }
    public double? HatchRate { get; set; }
    public double? SurvivalRate { get; set; }
    public double? HighQualifiedRate { get; set; }
    }
}

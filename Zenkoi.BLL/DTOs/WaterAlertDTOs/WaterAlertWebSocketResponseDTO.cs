using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.WaterAlertDTOs
{
    public class WaterAlertWebSocketResponseDTO
    { 
    public int PondId { get; set; }
    public string PondName { get; set; }
    public string ParameterName { get; set; }
    public double MeasuredValue { get; set; }
    public string AlertType { get; set; }
    public string Severity { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsResolved { get; set; }
}
}
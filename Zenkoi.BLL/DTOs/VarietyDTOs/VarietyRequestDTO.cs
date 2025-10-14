using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.VarietyDTOs
{
    public class VarietyRequestDTO
    {
        [Required(ErrorMessage = "VarietyName is required.")]
        [MaxLength(100, ErrorMessage = "VarietyName cannot exceed 100 characters.")]
        public string VarietyName { get; set; }

        [MaxLength(500, ErrorMessage = "Characteristic cannot exceed 500 characters.")]
        public string Characteristic { get; set; }

        [MaxLength(100, ErrorMessage = "OriginCountry cannot exceed 100 characters.")]
        public string OriginCountry { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.PatternDTOs
{
    public class PatternRequestDTO
    {
        [Required(ErrorMessage = "Pattern name is required.")]
        [StringLength(100, ErrorMessage = "Pattern name cannot exceed 100 characters.")]
        public string PatternName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }
    }
}

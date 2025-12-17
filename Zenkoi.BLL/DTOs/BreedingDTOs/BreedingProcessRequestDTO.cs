using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.BreedingDTOs
{
    public class BreedingProcessRequestDTO
    {
        [Required(ErrorMessage = "vui lòng chọn cá trống")]
        public int MaleKoiId { get; set; }
        [Required(ErrorMessage ="vui lòng chọn cá mái") ]
        public int FemaleKoiId { get; set; }
        [Required(ErrorMessage = "vui lòng chọn hồ")]
        public int PondId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.KoiFishDTOs
{
    public class KoiFishFamilyResponseDTO
    {
        public int Id { get; set; }
        public string RFID { get; set; }
        public string VarietyName { get; set; }
        public Gender Gender { get; set; }

        // Cha
        public KoiParentDTO Father { get; set; }

        // Mẹ
        public KoiParentDTO Mother { get; set; }
    }

    public class KoiParentDTO
    {
        public int Id { get; set; }
        public string RFID { get; set; }
        public string VarietyName { get; set; }
        public Gender Gender { get; set; }

  
        public KoiGrandParentDTO Father { get; set; }

        public KoiGrandParentDTO Mother { get; set; }
    }

    public class KoiGrandParentDTO
    {
        public int Id { get; set; }
        public string RFID { get; set; }
        public string VarietyName { get; set; }
        public Gender Gender { get; set; }
    }
}

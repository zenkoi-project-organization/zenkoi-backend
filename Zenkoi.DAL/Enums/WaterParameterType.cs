using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Zenkoi.DAL.Enums
{
    public enum WaterParameterType
    {
        PHLevel,            // Độ pH
        TemperatureCelsius, // Nhiệt độ (°C)
        OxygenLevel,        // Hàm lượng Oxy hòa tan (mg/L)
        AmmoniaLevel,       // Nồng độ Amoniac (mg/L)
        NitriteLevel,       // Nồng độ Nitrit (mg/L)
        NitrateLevel,       // Nồng độ Nitrat (mg/L)
        CarbonHardness,     // Độ cứng cacbonat (KH)
        WaterLevelMeters    // Mực nước (m)
    }
}

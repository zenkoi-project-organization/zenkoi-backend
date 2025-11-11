using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Enums
{
    public enum FishSize
    {
        From0To19cm,            // 0 - 19 cm (0 - 7.48 inch)
        From20To25cm,           // 20 - 25 cm (7.87 - 9.84 inch)
        From25_1To30cm,         // 25.1 - 30 cm (9.88 - 11.81 inch)
        From30_1To40cm,         // 30.1 - 40 cm (11.85 - 15.75 inch)
        From40_1To44cm,         // 40.1 - 44 cm (15.79 - 17.32 inch)
        From44_1To50cm,         // 44.1 - 50 cm (17.36 - 19.69 inch)
        From50_1To55cm,         // 50.1 - 55 cm (19.72 - 21.65 inch)
        From50_1To60cm_Hirenaga, // 50.1 - 60 cm Hirenaga/Butterfly (19.72 - 23.62 inch)
        From55_1To60cm,         // 55.1 - 60 cm (21.69 - 23.62 inch)
        From60_1To65cm,         // 60.1 - 65 cm (23.66 - 25.59 inch)
        From60_1To65cm_Hirenaga, // 60.1 - 65 cm Hirenaga/Butterfly (23.66 - 25.59 inch)
        From65_1To73cm,         // 65.1 - 73 cm (25.63 - 28.74 inch)
        From73_1To83cm,         // 73.1 - 83 cm (28.78 - 32.68 inch)
        Over83_1cm              // 83.1+ cm (32.72+ inch)
    }
}

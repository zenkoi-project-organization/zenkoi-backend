using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Enums
{
    public enum TypeOfPond
    {
        Paring,
        EggBatch,
        FryFish,
        Classification, 
        MarketPond,  // hồ nuôi cá bán 
        GrowOut,   // Hồ nuôi cá lớn 
        BroodStock,  // Hồ nuôi cá bố mẹ 
        Quarantine   // Hồ cách ly
    }
}

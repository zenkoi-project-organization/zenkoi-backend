using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Enums
{
    [Flags]
    public enum MutationType
    {
        None = 0,
        Doitsu = 1 << 0  ,        // Koi không vảy
        GinRin = 1 << 1 ,        // Vảy ánh kim 
        Hirenaga = 1 << 2 ,      // Đuôi dài (Butterfly Koi)
        Metallic = 1<<3 ,      // Ánh kim toàn thân 
    }
}

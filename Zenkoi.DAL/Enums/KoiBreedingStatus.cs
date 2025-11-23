using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Enums
{
    public enum KoiBreedingStatus
    {
        NotMature,
        Ready,          // Sẵn sàng ghép cặp
        Spawning,       // Đang sinh sản
        PostSpawning    // Sau sinh sản, đang hồi phục
}
}

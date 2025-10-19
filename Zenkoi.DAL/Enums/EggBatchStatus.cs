using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Enums
{
    public enum EggBatchStatus
    {
        Collected,      // Lô trứng vừa được thu từ quá trình sinh sản   
        Incubating,      // Đang được ấp
        PartiallyHatched,// Một phần trứng đã nở
        Success,
        Failed
    }
}

using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum SaleStatus
    {
        [EnumMember(Value = "Không bán")]
        NotForSale = 1,
        [EnumMember(Value = "Có thể bán")]
        Available = 2,
        [EnumMember(Value = "Đã bán")]
        Sold = 3,
        [EnumMember(Value = "Đang chờ thanh toán")]
        PendingSale = 4
    }
}


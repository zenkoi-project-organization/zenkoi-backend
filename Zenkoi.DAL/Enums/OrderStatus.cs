using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending = 0,
        [EnumMember(Value = "Processing")]
        Processing = 1,
        [EnumMember(Value = "UnShipping")]
        UnShiping = 2,
        [EnumMember(Value = "Shipped")]
        Shipped = 3,
        [EnumMember(Value = "Cancelled")]
        Cancelled = 4,
        [EnumMember(Value = "Rejected")]
        Rejected = 5,
        [EnumMember(Value = "Delivered")]
        Delivered = 6,
        [EnumMember(Value = "Refund")]
        Refund = 7

    }
}

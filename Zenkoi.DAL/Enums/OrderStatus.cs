using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Created")]
        Created = 0,
        [EnumMember(Value = "Confirmed")]
        Confirmed = 1,
        [EnumMember(Value = "Shipped")]
        Shipped = 2,
        [EnumMember(Value = "Delivered")]
        Delivered = 3,
        [EnumMember(Value = "Cancelled")]
        Cancelled = 4,
        [EnumMember(Value = "Completed")]
        Completed = 5
    }
}

using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum OrderStatus
    {
        [EnumMember(Value = "PendingPayment")]
        PendingPayment = 0,
        [EnumMember(Value = "Paid")]
        Paid = 1,
        [EnumMember(Value = "Confirmed")]
        Confirmed = 2,
        [EnumMember(Value = "Shipped")]
        Shipped = 3,
        [EnumMember(Value = "Cancelled")]
        Cancelled = 4,
        [EnumMember(Value = "Completed")]
        Completed = 5
    }
}

using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Created")]
        Created = 0,
        [EnumMember(Value = "PendingPayment")]
        PendingPayment = 1,
        [EnumMember(Value = "Paid")]
        Paid = 2,
        [EnumMember(Value = "Confirmed")]
        Confirmed = 3,
        [EnumMember(Value = "Shipped")]
        Shipped = 4,
        [EnumMember(Value = "Cancelled")]
        Cancelled = 5,
        [EnumMember(Value = "Completed")]
        Completed = 6
    }
}

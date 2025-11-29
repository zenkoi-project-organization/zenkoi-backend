using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum PaymentTransactionStatus
    {
        [EnumMember(Value = "Pending")]
        Pending = 0,
        [EnumMember(Value = "Success")]
        Success = 1,
        [EnumMember(Value = "Failed")]
        Failed = 2,
        [EnumMember(Value = "Cancelled")]
        Cancelled = 3
    }
}

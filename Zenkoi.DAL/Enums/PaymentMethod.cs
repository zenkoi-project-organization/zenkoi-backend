using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum PaymentMethod
    {
        [EnumMember(Value = "Unknown")]
        Unknown = 0,
        [EnumMember(Value = "VNPAY")]
        VNPAY = 1,
        [EnumMember(Value = "PayOS")]
        PayOS = 2,
        [EnumMember(Value = "PayPal")]
        PayPal = 3,
        [EnumMember(Value = "Cash")]
        Cash = 4,
        [EnumMember(Value = "BankTransfer")]
        BankTransfer = 5
    }
}

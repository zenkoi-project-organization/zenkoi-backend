using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum ShippingRuleType
    {
        [EnumMember(Value = "ByAge")]
        ByAge = 1,
        [EnumMember(Value = "ByMaxLength")]
        ByMaxLength = 2,
        [EnumMember(Value = "ByCount")]
        ByCount = 3,
        [EnumMember(Value = "ByWeight")]
        ByWeight = 4
    }
}
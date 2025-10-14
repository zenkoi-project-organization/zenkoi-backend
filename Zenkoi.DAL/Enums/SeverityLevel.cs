using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum SeverityLevel
    {
        [EnumMember(Value = "Low")]
        Low = 0,
        [EnumMember(Value = "Medium")]
        Medium = 1,
        [EnumMember(Value = "High")]
        High = 2,
        [EnumMember(Value = "Urgent")]
        Urgent = 3
    }
}

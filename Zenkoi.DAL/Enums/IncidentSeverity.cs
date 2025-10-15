using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum IncidentSeverity
    {
        [EnumMember(Value = "Low")]
        Low = 0,
        [EnumMember(Value = "Medium")]
        Medium = 1,
        [EnumMember(Value = "High")]
        High = 2,
        [EnumMember(Value = "Critical")]
        Critical = 3
    }
}

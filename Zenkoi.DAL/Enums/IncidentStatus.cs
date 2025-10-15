

using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum IncidentStatus
    {
        [EnumMember(Value = "Reported")]
        Reported = 0,
        [EnumMember(Value = "Investigating")]
        Investigating = 1,
        [EnumMember(Value = "Resolved")]
        Resolved = 2,
        [EnumMember(Value = "Closed")]
        Closed = 3,
        [EnumMember(Value = "Cancelled")]
        Cancelled = 4
    }
}

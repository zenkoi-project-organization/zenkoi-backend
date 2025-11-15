

using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum IncidentStatus
    {
        [EnumMember(Value = "Reported")]
        Reported = 0,   
        [EnumMember(Value = "Resolved")]
        Resolved = 1,
        [EnumMember(Value = "Cancelled")]
        Cancelled = 2
    }
}

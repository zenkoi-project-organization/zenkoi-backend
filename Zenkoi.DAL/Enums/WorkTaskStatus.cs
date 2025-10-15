using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum WorkTaskStatus
    {
        [EnumMember(Value = "Pending")]
        Pending = 0,
        [EnumMember(Value = "InProgress")]
        InProgress = 1,
        [EnumMember(Value = "Completed")]
        Completed = 2,
        [EnumMember(Value = "Cancelled")]
        Cancelled = 3
    }
}

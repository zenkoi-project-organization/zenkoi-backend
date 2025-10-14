

using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum KoiAffectedStatus
    {
        [EnumMember(Value = "Exposed")]
        Exposed = 0,     // Có khả năng bị ảnh hưởng
        [EnumMember(Value = "Infected")]
        Infected = 1,    // Đã nhiễm bệnh
        [EnumMember(Value = "UnderTreatment")]
        UnderTreatment = 2,
        [EnumMember(Value = "Recovered")]
        Recovered = 3,
        [EnumMember(Value = "Deceased")]
        Deceased = 4
    }
}

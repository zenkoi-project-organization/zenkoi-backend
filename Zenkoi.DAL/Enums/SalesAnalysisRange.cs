using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum SalesAnalysisRange
    {
        [EnumMember(Value = "30d")]
        Last30Days = 0,
        
        [EnumMember(Value = "12m")]
        Last12Months = 1
    }
}


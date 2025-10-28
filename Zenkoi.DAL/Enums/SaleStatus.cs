using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
    public enum SaleStatus
    {
        [EnumMember(Value = "Không bán")]
        NotForSale = 1,   
        [EnumMember(Value = "Có thể bán")]
        Available = 2,    
        [EnumMember(Value = "Đã được đặt")]
        Reserved = 3,      
        [EnumMember(Value = "Đã bán")]
        Sold = 4           
    }
}


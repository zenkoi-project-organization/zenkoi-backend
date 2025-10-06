using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
	public enum Role
	{
		[EnumMember(Value = "Quản lý")]
		Manager = 1,
		[EnumMember(Value = "Farm staff")]
		FarmStaff = 2,
		[EnumMember(Value = "Sale staff")]
		SaleStaff = 3,
        [EnumMember(Value = "Customer")]
        Customer = 4
    }
}

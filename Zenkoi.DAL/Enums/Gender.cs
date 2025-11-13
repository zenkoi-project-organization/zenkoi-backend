using System.Runtime.Serialization;

namespace Zenkoi.DAL.Enums
{
	public enum Gender
	{
		[EnumMember(Value = "Male")]
		Male,
		[EnumMember(Value = "Female")]
		Female,
		[EnumMember(Value = "Other")]
		Other
	}
}

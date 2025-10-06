using System.Reflection;
using System.Runtime.Serialization;

namespace Zenkoi.BLL.Helpers.Enum
{
	public static class EnumHelper
	{
		public static string GetEnumMemberValue<T>(T enumValue) where T : System.Enum
		{
			var field = typeof(T).GetField(enumValue.ToString());
			var attribute = field?.GetCustomAttribute<EnumMemberAttribute>();
			return attribute?.Value ?? enumValue.ToString();
		}

		public static List<object> GetEnumList<T>() where T : System.Enum
		{
			return System.Enum.GetValues(typeof(T))
				.Cast<T>()
				.Select(e => new
				{
					Value = Convert.ToInt32(e),
					Name = e.ToString(),
					DisplayName = GetEnumMemberValue(e)
				})
				.ToList<object>();
		}
	}
}

using System.Linq.Expressions;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.Helpers.Fillters
{
	public static class FilterHelper
	{
		public static Expression<Func<T, bool>> BuildSearchExpression<T>(string search)
		{
			var parameter = Expression.Parameter(typeof(T), "x");
			Expression expression = Expression.Constant(false);

			foreach (var property in typeof(T).GetProperties())
			{
				var propertyType = property.PropertyType;
				var propertyAccess = Expression.Property(parameter, property);

				if (propertyType == typeof(string))
				{
					var searchValue = Expression.Constant(search.ToLower().Trim());
					var toLowerMethod = typeof(string).GetMethod("ToLower", new Type[] { });
					var containsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });

					var toLowerExpression = Expression.Call(propertyAccess, toLowerMethod);
					var containsExpression = Expression.Call(toLowerExpression, containsMethod, searchValue);

					expression = Expression.OrElse(expression, containsExpression);
				}
				else if (propertyType == typeof(int) ||
						 propertyType == typeof(long) ||
						 propertyType == typeof(decimal) ||
						 propertyType == typeof(float) ||
						 propertyType == typeof(double) ||
						 propertyType == typeof(byte) ||
						 propertyType == typeof(short))
				{
					bool parsed = false;
					object numberValue = null;

					if (propertyType == typeof(int))
						parsed = int.TryParse(search, out int intValue) && (numberValue = intValue) != null;
					else if (propertyType == typeof(long))
						parsed = long.TryParse(search, out long longValue) && (numberValue = longValue) != null;
					else if (propertyType == typeof(decimal))
						parsed = decimal.TryParse(search, out decimal decimalValue) && (numberValue = decimalValue) != null;
					else if (propertyType == typeof(float))
						parsed = float.TryParse(search, out float floatValue) && (numberValue = floatValue) != null;
					else if (propertyType == typeof(double))
						parsed = double.TryParse(search, out double doubleValue) && (numberValue = doubleValue) != null;
					else if (propertyType == typeof(byte))
						parsed = byte.TryParse(search, out byte byteValue) && (numberValue = byteValue) != null;
					else if (propertyType == typeof(short))
						parsed = short.TryParse(search, out short shortValue) && (numberValue = shortValue) != null;

					if (parsed)
					{
						var searchValue = Expression.Constant(numberValue);
						var equalExpression = Expression.Equal(propertyAccess, searchValue);
						expression = Expression.OrElse(expression, equalExpression);
					}
				}
				else if (propertyType == typeof(DateTime))
				{
					if (DateTime.TryParse(search, out DateTime dateTimeValue))
					{
						var searchValue = Expression.Constant(dateTimeValue);
						var equalExpression = Expression.Equal(propertyAccess, searchValue);
						expression = Expression.OrElse(expression, equalExpression);
					}
				}
				else if (propertyType == typeof(bool))
				{
					if (bool.TryParse(search, out bool boolValue))
					{
						var searchValue = Expression.Constant(boolValue);
						var equalExpression = Expression.Equal(propertyAccess, searchValue);
						expression = Expression.OrElse(expression, equalExpression);
					}
				}
				else if (propertyType.IsEnum)
				{
					try
					{
						var enumValue = System.Enum.Parse(propertyType, search, ignoreCase: true);
						var searchValue = Expression.Constant(enumValue, propertyType);
						var equalExpression = Expression.Equal(propertyAccess, searchValue);
						expression = Expression.OrElse(expression, equalExpression);
					}
					catch (ArgumentException ex)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine(ex.Message);
						Console.ResetColor();
					}
				}
			}

			return Expression.Lambda<Func<T, bool>>(expression, parameter);
		}
        public static Expression<Func<KoiFish, bool>> BuildFilterExpression(KoiFishFilterRequestDTO filter)
        {
            var parameter = Expression.Parameter(typeof(KoiFish), "x");
            Expression finalExpr = Expression.Constant(true);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchExpr = BuildSearchExpression<KoiFish>(filter.Search).Body;
                finalExpr = Expression.AndAlso(finalExpr, searchExpr);
            }

            if (filter.Gender.HasValue)
            {
                var prop = Expression.Property(parameter, nameof(KoiFish.Gender));
                var equalExpr = Expression.Equal(prop, Expression.Constant(filter.Gender.Value));
                finalExpr = Expression.AndAlso(finalExpr, equalExpr);
            }

            // ❤️ Health
            if (filter.Health.HasValue)
            {
                var prop = Expression.Property(parameter, nameof(KoiFish.HealthStatus));
                var equalExpr = Expression.Equal(prop, Expression.Constant(filter.Health.Value));
                finalExpr = Expression.AndAlso(finalExpr, equalExpr);
            }

            if (filter.VarietyId.HasValue)
            {
                var prop = Expression.Property(parameter, nameof(KoiFish.VarietyId));
                var equalExpr = Expression.Equal(prop, Expression.Constant(filter.VarietyId.Value));
                finalExpr = Expression.AndAlso(finalExpr, equalExpr);
            }

            if (filter.PondId.HasValue)
            {
                var prop = Expression.Property(parameter, nameof(KoiFish.PondId));
                var equalExpr = Expression.Equal(prop, Expression.Constant(filter.PondId.Value));
                finalExpr = Expression.AndAlso(finalExpr, equalExpr);
            }

        
            if (filter.MinPrice.HasValue)
            {
                var prop = Expression.Property(parameter, nameof(KoiFish.SellingPrice));
                var geExpr = Expression.GreaterThanOrEqual(prop, Expression.Constant(filter.MinPrice.Value));
                finalExpr = Expression.AndAlso(finalExpr, geExpr);
            }

            if (filter.MaxPrice.HasValue)
            {
                var prop = Expression.Property(parameter, nameof(KoiFish.SellingPrice));
                var leExpr = Expression.LessThanOrEqual(prop, Expression.Constant(filter.MaxPrice.Value));
                finalExpr = Expression.AndAlso(finalExpr, leExpr);
            }


            return Expression.Lambda<Func<KoiFish, bool>>(finalExpr, parameter);
        }
    }
}
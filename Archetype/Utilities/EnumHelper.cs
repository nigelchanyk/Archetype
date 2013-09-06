using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype.Utilities
{
	public static class EnumHelper
	{
		public static IEnumerable<T> GetValues<T>()
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

		public static T ParseAsEnum<T>(this string value, bool ignoreCase = false) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("Given type is not a enum.");

			T t;
			if (!Enum.TryParse<T>(value, ignoreCase, out t))
				throw new InvalidOperationException(value + " cannot be parsed as enum.");

			return t;
		}
	}
}

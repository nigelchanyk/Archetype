using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype.Utilities
{
	public static class ContainerHelper
	{
		public static Nullable<U> MinOrNull<T, U>(this IEnumerable<T> container, Func<T, U?> func) where U : struct, IComparable
		{
			U? min = null;
			foreach (T element in container)
			{
				U? result = func(element);
				if (min == null || (result != null && min.Value.CompareTo(result.Value) < 0))
					min = result;
			}

			return min;
		}
	}
}

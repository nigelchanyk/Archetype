using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype.Utilities
{
	public static class ContainerHelper
	{
		private static readonly Random Randomizer = new Random();

		public static IndexedEnumerator<T> IndexedEnumerator<T>(this IEnumerable<T> enumerable)
		{
			return new IndexedEnumerator<T>(enumerable);
		}

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

		public static IEnumerable<T> Slice<T>(this T[] array, int start, int end, int step = 1)
		{
			for (int i = start; i < end; i += step)
				yield return array[i];
		}

		public static void Swap<T>(this List<T> list, int index, int index2)
		{
			T temp = list[index];
			list[index] = list[index2];
			list[index2] = temp;
		}

		public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> enumerable, int count)
		{
			List<T> list = enumerable.ToList();
			for (int i = list.Count - 1; i >= 1; --i)
				list.Swap(i, Randomizer.Next(i + 1));

			return list.Take(count);
		}
	}
}

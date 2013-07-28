using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Archetype.Utilities
{
	public static class BinarySearch
	{
		/// <summary>
		/// Perform binary search on the given vector.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <param name="iteration">Number of iterations.</param>
		/// <param name="condition">Return true to approach target.</param>
		/// <returns></returns>
		public static Vector3 Iterate(Vector3 source, Vector3 target, int iteration, Func<Vector3, bool> condition)
		{
			bool rangeIncreasing = true;
			Vector3 min = source;
			Vector3 max = target;
			Vector3 mid = (min + max) / 2;
			for (int i = 0; i < iteration; ++i)
			{
				if (condition(mid))
					min = mid;
				else
				{
					max = mid;
					rangeIncreasing = false;
				}
				mid = (min + max) / 2;
			}

			// Edge case for maximum value.
			if (rangeIncreasing && condition(max))
				return max;

			return min;
		}
	}
}

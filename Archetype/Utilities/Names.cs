using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Archetype.Utilities
{
	public static class Names
	{
		private static readonly Random Randomizer = new Random();
		private static List<string> NameList;

		public static void Initialize()
		{
			NameList = File.ReadLines("StringResources/Names.txt").ToList();
		}

		public static IEnumerable<string> Random(int count)
		{
			return NameList.TakeRandom(count);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Archetype.Utilities
{
	public static class Names
	{
		private static readonly Random Randomizer = new Random();
		private static List<string> NameList = new List<string>();

		public static void Initialize()
		{
			NameList.Clear();
			using (StreamReader sr = new StreamReader("StringResources/Names.txt"))
			{
				NameList.Add(sr.ReadLine());
			}
		}

		public static IEnumerable<string> Random(int count)
		{
			return NameList.OrderBy(x => Randomizer.Next()).Take(count);
		}
	}
}

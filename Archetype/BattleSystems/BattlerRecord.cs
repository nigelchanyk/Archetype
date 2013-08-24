using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archetype.Utilities;
using Archetype.Objects.Characters;

namespace Archetype.BattleSystems
{
	public class BattlerRecord
	{
		public Character Character { get; set; }
		public int DeathCount { get; set; }
		public int KillCount { get; set; }
		public string Name { get; private set; }

		public BattlerRecord(string name)
		{
			this.Name = name;
		}
	}
}

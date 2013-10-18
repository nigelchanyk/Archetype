using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Objects.Characters;
using Archetype.Utilities;

namespace Archetype.BattleSystems
{
	public class BattlerRecord
	{
		public event EventHandler BattlerDead;

		public Character Character { get; set; }
		public int DeathCount { get; private set; }
		public int KillCount { get; private set; }
		public string Name { get; private set; }

		public BattlerRecord(string name)
		{
			this.Name = name;
		}

		public void NotifyDead()
		{
			DeathCount++;
			if (BattlerDead != null)
				BattlerDead(this, new EventArgs());
		}
	}
}

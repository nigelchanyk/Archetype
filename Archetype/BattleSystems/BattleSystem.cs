using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archetype.Objects;
using Archetype.Objects.Characters;

namespace Archetype.BattleSystems
{
	public abstract class BattleSystem
	{
		public World World { get; set; }
		protected Dictionary<string, BattlerRecord> BattlerNameMapper { get; private set; }

		public BattleSystem()
		{
			BattlerNameMapper = new Dictionary<string, BattlerRecord>();
		}

		public abstract Character GetCharacterByName(string name);
		public abstract IEnumerable<Character> GetEnemiesAlive(Character character);
		public abstract void Start();
	}
}

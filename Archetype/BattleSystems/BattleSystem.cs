﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Objects;
using Archetype.Objects.Characters;
using Archetype.Utilities;
using Archetype.Events;

namespace Archetype.BattleSystems
{
	public abstract class BattleSystem
	{
		public enum Kind
		{
			TeamBattle
		}

		public event EventHandler BattleEnded;

		public string Message { get; private set; }
		public World World { get; set; }

		protected Dictionary<string, BattlerRecord> BattlerNameMapper { get; private set; }

		public BattleSystem()
		{
			BattlerNameMapper = new Dictionary<string, BattlerRecord>();
		}

		public IEnumerable<BattlerRecord> GetAllRecords()
		{
			return BattlerNameMapper.Values;
		}

		public Character GetCharacterByName(string name)
		{
			BattlerRecord record = BattlerNameMapper.Get(name, null);
			return record == null ? null : record.Character;
		}

		public abstract IEnumerable<Character> GetEnemiesAlive(Character character);
		public abstract void Start();

		protected void NotifyBattleEnded(string message)
		{
			this.Message = message;
			if (BattleEnded != null)
				BattleEnded(this, new EventArgs());
		}

		protected abstract void OnBattlerDead(object sender, EventArgs e);
	}
}

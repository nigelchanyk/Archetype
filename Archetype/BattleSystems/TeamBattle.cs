using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Objects;
using Archetype.Utilities;
using Archetype.Objects.Characters;

namespace Archetype.BattleSystems
{
	public class TeamBattle : BattleSystem
	{
		public enum Team
		{
			Red,
			Blue
		}

		private Dictionary<Team, List<BattlerRecord>> TeamRecordMapper = new Dictionary<Team, List<BattlerRecord>>();
		private Dictionary<Team, List<BattlerRecord>> OpposingTeamRecordMapper = new Dictionary<Team, List<BattlerRecord>>();
		private Dictionary<BattlerRecord, Team> TeamMapper = new Dictionary<BattlerRecord, Team>();

		public TeamBattle()
		{
			List<BattlerRecord> redList = new List<BattlerRecord>();
			List<BattlerRecord> blueList = new List<BattlerRecord>();

			TeamRecordMapper.Add(Team.Red, redList);
			TeamRecordMapper.Add(Team.Blue, blueList);
			OpposingTeamRecordMapper.Add(Team.Blue, redList);
			OpposingTeamRecordMapper.Add(Team.Red, blueList);
		}

		public void AddBots(int redBotCount, int blueBotCount)
		{
			string[] names = Names.Random(redBotCount + blueBotCount).ToArray();
			foreach (string name in names.Slice(0, redBotCount))
				TeamRecordMapper[Team.Red].Add(new BattlerRecord(name));
			foreach (string name in names.Slice(redBotCount, names.Length))
				TeamRecordMapper[Team.Blue].Add(new BattlerRecord(name));

		}

		public void AddPlayer(string name, Team team)
		{
			TeamRecordMapper[team].Add(new BattlerRecord(name));
		}

		public override Character GetCharacterByName(string name)
		{
			foreach (List<BattlerRecord> team in TeamRecordMapper.Values)
			{
				BattlerRecord record = team.FirstOrDefault(x => x.Name == name);
				if (record.Character != null)
					return record.Character;
			}
			return null;
		}

		public override IEnumerable<Character> GetEnemiesAlive(Character character)
		{
			if (character.Record == null)
				throw new ArgumentException("Character does not have any battler records.");
			if (!TeamMapper.ContainsKey(character.Record))
				throw new ArgumentException("Record does not exist in the battle system.");

			return from record in OpposingTeamRecordMapper[TeamMapper[character.Record]]
				   where record.Character != null && record.Character.Alive
				   select record.Character;
		}

		public override void Start()
		{
			foreach (List<BattlerRecord> team in TeamRecordMapper.Values)
			{
				foreach (BattlerRecord record in team)
				{
					record.Character = World.CreateCharacter();
					record.Character.Record = record;
				}
			}
		}
	}
}

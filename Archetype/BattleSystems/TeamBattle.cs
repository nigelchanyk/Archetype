using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.DataLoaders;
using Archetype.Objects;
using Archetype.Objects.Characters;
using Archetype.Utilities;

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
			{
				BattlerRecord record = new BattlerRecord(name);
				TeamRecordMapper[Team.Red].Add(record);
				TeamMapper.Add(record, Team.Red);
			}
			foreach (string name in names.Slice(redBotCount, names.Length))
			{
				BattlerRecord record = new BattlerRecord(name);
				TeamRecordMapper[Team.Blue].Add(record);
				TeamMapper.Add(record, Team.Blue);
			}

		}

		public void AddPlayer(string name, Team team)
		{
			BattlerRecord record = new BattlerRecord(name);
			TeamRecordMapper[team].Add(record);
			TeamMapper.Add(record, team);
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
			foreach (Team team in TeamRecordMapper.Keys)
			{
				List<BattlerRecord> teamRecord = TeamRecordMapper[team];
				IEnumerable<SpawnPoint> spawnPoints = GenerateSpawnPoints(
					WorldConfigurationLoader.GetConfiguration(World.SceneName).TeamBattleConfiguration,
					team,
					teamRecord.Count
				);
				foreach (var itr in teamRecord.Zip(spawnPoints, (r, p) => new { TeamRecord = r, SpawnPoint = p }))
				{
					itr.TeamRecord.Character = World.CreateCharacter();
					itr.TeamRecord.Character.Record = itr.TeamRecord;
					itr.TeamRecord.Character.Position = itr.SpawnPoint.ToVector3();
				}
			}
		}

		private IEnumerable<SpawnPoint> GenerateSpawnPoints(WorldConfigurationLoader.TeamBattleConfiguration configuration, Team team, int count)
		{
			return configuration.GetSpawnPoints(team).TakeRandom(count);
		}
	}
}

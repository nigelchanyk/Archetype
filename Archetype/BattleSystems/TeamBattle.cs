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
		private Dictionary<BattlerRecord, Team> TeamMapper = new Dictionary<BattlerRecord, Team>();

		public TeamBattle()
		{
			List<BattlerRecord> redList = new List<BattlerRecord>();
			List<BattlerRecord> blueList = new List<BattlerRecord>();

			TeamRecordMapper.Add(Team.Red, redList);
			TeamRecordMapper.Add(Team.Blue, blueList);
		}

		public void AddBots(int redBotCount, int blueBotCount)
		{
			string[] names = Names.Random(redBotCount + blueBotCount).ToArray();
			foreach (string name in names.Slice(0, redBotCount))
				GenerateBattleRecord(name, Team.Red);
			foreach (string name in names.Slice(redBotCount, names.Length))
				GenerateBattleRecord(name, Team.Blue);
		}

		public void AddPlayer(string name, Team team)
		{
			GenerateBattleRecord(name, team);
		}

		public override IEnumerable<Character> GetEnemiesAlive(Character character)
		{
			if (character.Record == null)
				throw new ArgumentException("Character does not have any battler records.");
			if (!TeamMapper.ContainsKey(character.Record))
				throw new ArgumentException("Record does not exist in the battle system.");

			return from record in TeamRecordMapper[GetOpposingTeam(TeamMapper[character.Record])]
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
					if (itr.TeamRecord.Character == null)
					{
						itr.TeamRecord.Character = World.CreateCharacter();
						itr.TeamRecord.Character.Record = itr.TeamRecord;
					}
					itr.TeamRecord.Character.Position = itr.SpawnPoint.ToVector3();
					itr.TeamRecord.Character.ResetHealth();
				}
			}
		}

		protected override void OnBattlerDead(object sender, EventArgs e)
		{
			foreach (KeyValuePair<Team, List<BattlerRecord>> pair in TeamRecordMapper)
			{
				if (pair.Value.Any(x => x.Character.Alive))
					continue;

				Team winningTeam = GetOpposingTeam(pair.Key);
				NotifyBattleEnded(winningTeam.ToString() + " wins the match!");
				return;
			}
		}

		private void GenerateBattleRecord(string name, Team team)
		{
			BattlerRecord record = new BattlerRecord(name);
			record.BattlerDead += OnBattlerDead;
			TeamRecordMapper[team].Add(record);
			TeamMapper.Add(record, team);
			BattlerNameMapper.Add(name, record);
		}

		private IEnumerable<SpawnPoint> GenerateSpawnPoints(WorldConfigurationLoader.TeamBattleConfiguration configuration, Team team, int count)
		{
			return configuration.GetSpawnPoints(team).TakeRandom(count);
		}

		private Team GetOpposingTeam(Team team)
		{
			return team == Team.Red ? Team.Blue : Team.Red;
		}
	}
}

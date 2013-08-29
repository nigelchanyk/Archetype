using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Archetype.BattleSystems;
using Archetype.Utilities;

namespace Archetype.DataLoaders
{
	public class WorldConfigurationLoader
	{
		private static readonly Dictionary<string, WorldConfiguration> ConfigurationMapper = new Dictionary<string, WorldConfiguration>();

		public static void Initialize()
		{
			XDocument doc = XDocument.Load("Assets/Data/WorldConfigurations.xml");
			foreach (XElement element in doc.Element("Worlds").Elements("World"))
			{
				WorldConfiguration configuration = WorldConfiguration.FromXML(element);
				ConfigurationMapper.Add(configuration.Name, configuration);
			}
		}

		public static WorldConfiguration GetConfiguration(string name)
		{
			return ConfigurationMapper[name];
		}

		public class WorldConfiguration
		{
			public string Name { get; private set; }
			public TeamBattleConfiguration TeamBattleConfiguration { get; private set; } // Nullable

			public static WorldConfiguration FromXML(XElement worldElement)
			{
				WorldConfiguration configuration = new WorldConfiguration();
				configuration.Name = worldElement.Attribute("name").Value;

				foreach (XElement battleElement in worldElement.Elements("BattleSystem"))
				{
					switch (battleElement.Attribute("name").Value)
					{
						case "TeamBattle":
							configuration.TeamBattleConfiguration = TeamBattleConfiguration.FromXML(battleElement);
							break;
					}
				}

				return configuration;
			}

			private WorldConfiguration()
			{
			}
		}

		public class TeamBattleConfiguration
		{
			private Dictionary<TeamBattle.Team, IEnumerable<SpawnPoint>> SpawnPointMapper = new Dictionary<TeamBattle.Team, IEnumerable<SpawnPoint>>();

			public static TeamBattleConfiguration FromXML(XElement element)
			{
				TeamBattleConfiguration configuration = new TeamBattleConfiguration();
				foreach (XElement teamElement in element.Elements("Team"))
				{
					TeamBattle.Team team = teamElement.Attribute("name").Value.ParseAsEnum<TeamBattle.Team>(true);
					// Call ToList() to avoid lazy loading
					IEnumerable<SpawnPoint> spawnPoints = teamElement.Elements("SpawnPoint").Select<XElement, SpawnPoint>(
						x => new SpawnPoint((float)x.Attribute("x"), (float)x.Attribute("z"))
					).ToList();
					configuration.SpawnPointMapper.Add(team, spawnPoints);
				}

				return configuration;
			}

			private TeamBattleConfiguration()
			{
			}

			public IEnumerable<SpawnPoint> GetSpawnPoints(TeamBattle.Team team)
			{
				return SpawnPointMapper[team];
			}
		}
	}
}

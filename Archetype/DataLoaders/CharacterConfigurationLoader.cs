using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Mogre;

using Archetype.Objects.Characters;
using Archetype.Utilities;

namespace Archetype.DataLoaders
{
	public static class CharacterConfigurationLoader
	{
		private static Dictionary<string, CharacterConfiguration> ConfigurationMapper = new Dictionary<string, CharacterConfiguration>();

		public static void Initialize()
		{
			XDocument doc = XDocument.Load("Assets/Data/CharacterConfigurations.xml");
			foreach (XElement characterElement in doc.Root.Elements("Character"))
			{
				string name = characterElement.Attribute("name").Value;
				IEnumerable<string> entityNames = from x in characterElement.Element("Entities").Elements("Entity")
												  select x.Attribute("name").Value;
				XElement thirdPersonElement = characterElement.Element("ThirdPerson");
				Dictionary<string, CharacterConfiguration.LocalSpacePosition> positionMapper = new Dictionary<string, CharacterConfiguration.LocalSpacePosition>();
				foreach (XElement weaponElement in thirdPersonElement.Element("Weapons").Elements("Weapon"))
				{
					Vector3 position = weaponElement.ParseXYZ(Vector3.ZERO);
					string weaponName = weaponElement.Attribute("name").Value;
					string world = weaponElement.Attribute("world").Value;
					positionMapper.Add(weaponName, new CharacterConfiguration.LocalSpacePosition(position, world));
				}

				ConfigurationMapper.Add(name, new CharacterConfiguration(name, entityNames.ToArray(), positionMapper));
			}
		}

		public static CharacterConfiguration Get(string name)
		{
			return ConfigurationMapper[name];
		}
	}
}

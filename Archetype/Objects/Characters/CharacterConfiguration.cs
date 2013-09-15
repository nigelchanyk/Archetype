using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.Objects.Characters
{
	public class CharacterConfiguration
	{
		public string[] EntityNames { get; private set; }
		public string Name { get; private set; }

		private Dictionary<string, LocalSpacePosition> ThirdPersonWeaponPositionMapper = new Dictionary<string, LocalSpacePosition>();

		public CharacterConfiguration(string name, string[] entityNames, Dictionary<string, LocalSpacePosition> thirdPersonWeaponPositionMapper)
		{
			this.Name = name;
			this.EntityNames = entityNames;
			this.ThirdPersonWeaponPositionMapper = thirdPersonWeaponPositionMapper;
		}

		public LocalSpacePosition GetThirdPersonWeaponPosition(string weaponName)
		{
			return ThirdPersonWeaponPositionMapper[weaponName];
		}


		public class LocalSpacePosition
		{
			public Vector3 Position { get; private set; }
			public string World { get; private set; }

			public LocalSpacePosition(Vector3 position, string world)
			{
				this.Position = position;
				this.World = world;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Objects;
using Archetype.Objects.Billboards;
using Archetype.Objects.Characters;
using Archetype.Utilities;
using Archetype.Events;

namespace Archetype.CompoundEffects
{
	public class MuzzleFlashEffect : IDisposable
	{
		private static readonly Vector3[] LightOffsets =
		{
			MathHelper.Left,
			MathHelper.Right,
			MathHelper.Up,
			MathHelper.Down
		};

		public bool Alive { get; private set; }
		public World World { get; private set; }

		private DecayableBillboard Billboard;
		private Light[] Lights = new Light[4];

		public MuzzleFlashEffect(World world, Character character, Vector3 weaponWorldPosition)
		{
			this.World = world;
			Billboard = World.CreateDecayableBillboard(BillboardSystemType.MuzzleFlash, character.ConvertWeaponToWorldPosition(weaponWorldPosition));

			for (int i = 0; i < Lights.Length; ++i)
			{
				Lights[i] = World.CreateLight(character.ConvertWeaponToWorldPosition(weaponWorldPosition + GetScaledOffset(LightOffsets[i])));
				Lights[i].Type = Light.LightTypes.LT_POINT;
				Lights[i].SetAttenuation(5, 0, 0, 0.5f);
				Lights[i].SetDiffuseColour(0.3f, 0.3f, 0.3f);
			}

			Alive = true;
		}

		public void Dispose()
		{
			if (Alive)
			{
				foreach (Light light in Lights)
					World.DestroyLight(light);

				Alive = false;
			}
		}

		public void Update(UpdateEvent evt)
		{
			if (!Billboard.Alive)
				Dispose();
		}

		private Vector3 GetScaledOffset(Vector3 offset)
		{
			return new Vector3(offset.x * Billboard.Dimension.x, offset.y * Billboard.Dimension.y, offset.z);
		}
	}
}

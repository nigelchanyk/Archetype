using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

using Archetype.Objects;
using Archetype.Objects.Characters;
using Archetype.Events;

namespace Archetype.CompoundEffects
{
	public class CompoundEffectManager : IDisposable
	{
		public World World { get; private set; }

		private HashSet<MuzzleFlashEffect> MuzzleFlashEffects = new HashSet<MuzzleFlashEffect>();

		public CompoundEffectManager(World world)
		{
			this.World = world;
		}

		public void CreateMuzzleFlashEffect(Character character, Vector3 weaponSpacePosition)
		{
			MuzzleFlashEffects.Add(new MuzzleFlashEffect(World, character, weaponSpacePosition));
		}

		public void Dispose()
		{
			foreach (MuzzleFlashEffect effect in MuzzleFlashEffects)
				effect.Dispose();
		}

		public void Update(UpdateEvent evt)
		{
			foreach (MuzzleFlashEffect effect in MuzzleFlashEffects)
				effect.Update(evt);

			MuzzleFlashEffects.RemoveWhere(x => !x.Alive);
		}
	}
}

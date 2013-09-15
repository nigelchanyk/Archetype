using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.Objects.Weapons
{
	public class RangedWeapon : Weapon
	{
		public float InaccuracyGrowth { get; private set; }
		public float MaxInaccuracy { get; private set; }
		public float MaxRecoil { get; private set; }
		public float MinInaccuracy { get; private set; }
		public Vector3 MuzzleFlashPosition { get; private set; }
		public float RecoilGrowth { get; private set; }

		public RangedWeapon(Kind kind, string name, string modelName, int baseDamage, float attackInterval, string attackSound, Vector3 firstPersonPosition,
			float minInaccuracy, float maxInaccuracy, float inaccuracyGrowth, float maxRecoil, float recoilGrowth, Vector3 MuzzleFlashPosition)
			: base(kind, name, modelName, baseDamage, attackInterval, attackSound, firstPersonPosition)
		{
			this.MinInaccuracy = minInaccuracy;
			this.MaxInaccuracy = maxInaccuracy;
			this.InaccuracyGrowth = inaccuracyGrowth;
			this.MaxRecoil = maxRecoil;
			this.RecoilGrowth = recoilGrowth;
			this.MuzzleFlashPosition = MuzzleFlashPosition;
		}
	}
}

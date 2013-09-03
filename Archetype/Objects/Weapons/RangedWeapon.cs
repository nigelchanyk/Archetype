using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype.Objects.Weapons
{
	public class RangedWeapon : Weapon
	{
		public float RecoilFactor { get; private set; }

		public RangedWeapon(Kind kind, string modelName, int baseDamage, float attackInterval, float recoilFactor)
			: base(kind, modelName, baseDamage, attackInterval)
		{
			this.RecoilFactor = recoilFactor;
		}
	}
}

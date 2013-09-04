using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archetype.Objects.Characters;
using Archetype.Objects.Weapons;
using Mogre;
using Archetype.Utilities;

namespace Archetype.Logic.WeaponHandlers
{
	public class RangedWeaponHandler : WeaponHandler
	{
		private static readonly SphericalCoordinate PreciseAttackDirection = MathHelper.Forward.ToSphericalCoordinate();

		public RangedWeapon RangedWeapon { get; private set; }

		public RangedWeaponHandler(Character actionPerformer, RangedWeapon weapon)
			: base(actionPerformer, weapon)
		{
			RangedWeapon = weapon;
			MinInaccuracy = RangedWeapon.MinInaccuracy;
		}

		protected override void OnRegularAttack()
		{
			SphericalCoordinate attackDirection = new SphericalCoordinate(
				PreciseAttackDirection.Radius,
				PreciseAttackDirection.Phi + RandomizeInaccuracy(),
				PreciseAttackDirection.Theta + RandomizeInaccuracy()
			);
			ActionPerformer.Attack(attackDirection.ToVector3(), Weapon.BaseDamage);
			Inaccuracy = System.Math.Min(RangedWeapon.MaxInaccuracy, Inaccuracy + RangedWeapon.InaccuracyGrowth);
		}

		private float RandomizeInaccuracy()
		{
			return MathHelper.Randomizer.NextFloat(-Inaccuracy, Inaccuracy);
		}
	}
}

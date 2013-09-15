using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Objects.Characters;
using Archetype.Objects.Weapons;
using Archetype.Utilities;
using Archetype.Objects.Billboards;

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
			ActionPerformer.IncreaseRecoil(RangedWeapon.RecoilGrowth, RangedWeapon.MaxRecoil);
			ActionPerformer.World.SoundEngine.Play3D("Assets/Audio/Gunshots/USP.ogg", ActionPerformer.EyeNode.GetWorldPosition());
			ActionPerformer.World.CreateMuzzleFlashEffect(ActionPerformer, RangedWeapon.MuzzleFlashPosition);
		}

		private float RandomizeInaccuracy()
		{
			return MathHelper.Randomizer.NextFloat(-Inaccuracy, Inaccuracy);
		}
	}
}

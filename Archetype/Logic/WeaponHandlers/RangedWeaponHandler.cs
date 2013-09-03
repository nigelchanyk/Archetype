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
		public RangedWeapon RangedWeapon { get { return (RangedWeapon)Weapon; } }

		public RangedWeaponHandler(Character actionPerformer, RangedWeapon weapon)
			: base(actionPerformer, weapon)
		{
		}

		protected override void OnRegularAttack()
		{
			ActionPerformer.Attack(MathHelper.Forward, Weapon.BaseDamage);
		}
	}
}

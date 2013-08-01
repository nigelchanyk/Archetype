using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archetype.Objects.Characters;
using Archetype.Objects.Weapons;

namespace Archetype.Logic.WeaponHandlers
{
	public abstract class RangedWeaponHandler : WeaponHandler
	{
		public RangedWeapon Weapon { get; private set; }

		public RangedWeaponHandler(Character actionPerformer, RangedWeapon weapon)
			: base(actionPerformer)
		{
			this.Weapon = weapon;
		}

		protected override void OnAttack()
		{
		}
	}
}

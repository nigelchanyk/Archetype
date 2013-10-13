using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.DataLoaders;
using Archetype.Handlers.ActionHandlers;
using Archetype.Handlers.SpecialMoveHandlers;
using Archetype.Handlers.WeaponHandlers;
using Archetype.Objects.Weapons;

namespace Archetype.Objects.Characters.Androids
{
	public class Assaulter : Android
	{
		protected override JumpHandler JumpHandler { get; set; }
		protected override WalkHandler WalkHandler { get; set; }

		public Assaulter(World world)
			: base(world)
		{
			JumpHandler = new JumpHandler(this, 4, GameConstants.DefaultGravityAcceleration);
			WalkHandler = new WalkHandler(this, GameConstants.DefaultWalkingSpeed);
			ActiveWeaponHandler = new RangedWeaponHandler(this, WeaponLoader.Get("USP"));
			SpecialMoveHandlers[0] = new PlasmaBeamHandler(this);
		}
	}
}

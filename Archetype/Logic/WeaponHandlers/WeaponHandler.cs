using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Events;
using Archetype.Objects.Characters;
using Archetype.Objects.Weapons;
using Archetype.Utilities;

namespace Archetype.Logic.WeaponHandlers
{
	public abstract class WeaponHandler
	{
		public Character ActionPerformer { get; private set; }
		public float Inaccuracy { get; protected set; }
		public float RemainingTime { get; protected set; }
		public Weapon Weapon { get; private set; }

		protected float MinInaccuracy { get; set; }

		public WeaponHandler(Character actionPerformer, Weapon weapon)
		{
			this.ActionPerformer = actionPerformer;
			this.Weapon = weapon;
		}

		public void RegularAttack()
		{
			if (RemainingTime <= 0)
			{
				OnRegularAttack();
				RemainingTime += Weapon.AttackInterval;
			}
		}

		public void Update(UpdateEvent evt)
		{
			RemainingTime = System.Math.Max(0, RemainingTime - evt.ElapsedTime);
			Inaccuracy = MathHelper.Lerp(Inaccuracy, MinInaccuracy, evt.ElapsedTime * GameConstants.InaccuracyReductionFactor);
			OnUpdate();
		}

		protected abstract void OnRegularAttack();
		protected virtual void OnUpdate() {}
	}
}

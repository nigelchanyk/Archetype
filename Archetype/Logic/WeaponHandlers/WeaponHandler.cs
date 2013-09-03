using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archetype.Objects.Characters;
using Archetype.Events;
using Archetype.Objects.Weapons;

namespace Archetype.Logic.WeaponHandlers
{
	public abstract class WeaponHandler
	{
		public Character ActionPerformer { get; private set; }
		public float CrosshairRadius { get; protected set; }
		public float RemainingTime { get; protected set; }
		public Weapon Weapon { get; private set; }

		public WeaponHandler(Character actionPerformer, Weapon weapon)
		{
			this.ActionPerformer = actionPerformer;
			this.Weapon = weapon;
		}

		public void RegularAttack()
		{
			if (RemainingTime <= 0)
				OnRegularAttack();
		}

		public void Update(UpdateEvent evt)
		{
			RemainingTime = System.Math.Max(0, RemainingTime - evt.ElapsedTime);
			OnUpdate();
		}

		protected abstract void OnRegularAttack();
		protected virtual void OnUpdate() {}
	}
}

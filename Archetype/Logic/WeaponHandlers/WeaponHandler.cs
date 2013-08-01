using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archetype.Objects.Characters;
using Archetype.Events;

namespace Archetype.Logic.WeaponHandlers
{
	public abstract class WeaponHandler
	{
		public Character ActionPerformer { get; private set; }
		public float RemainingTime { get; protected set; }

		public WeaponHandler(Character actionPerformer)
		{
			this.ActionPerformer = actionPerformer;
		}

		public void Attack()
		{
			if (RemainingTime <= 0)
				OnAttack();
		}

		public void Update(UpdateEvent evt)
		{
			RemainingTime = System.Math.Max(0, RemainingTime - evt.ElapsedTime);
			OnUpdate();
		}

		protected abstract void OnAttack();
		protected virtual void OnUpdate() {}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Events;
using Archetype.Objects.Characters;

namespace Archetype.Handlers.SpecialMoveHandlers
{
	public abstract class SpecialMoveHandler
	{
		public Character ActionPerformer { get; private set; }
		public float Interval { get; protected set; }
		public float RemainingTime { get; protected set; }

		public SpecialMoveHandler(Character actionPerformer, float interval)
		{
			this.ActionPerformer = actionPerformer;
			this.Interval = interval;
		}

		public void Trigger()
		{
			if (RemainingTime <= 0)
			{
				RemainingTime += Interval;
				OnTrigger();
			}
		}

		public void Update(UpdateEvent evt)
		{
			RemainingTime = Math.Max(0, RemainingTime - evt.ElapsedTime);
			OnUpdate(evt);
		}

		protected abstract void OnTrigger();
		protected virtual void OnUpdate(UpdateEvent evt) {}
	}
}

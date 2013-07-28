using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Events;
using Archetype.Objects.Characters;

namespace Archetype.Logic.ActionHandlers
{
	public class ActionHandler
	{
		public Character ActionPerformer { get; private set; }

		public ActionHandler(Character actionPerformer)
		{
			this.ActionPerformer = actionPerformer;
		}

		public virtual Vector3 GetVelocityInfluence(UpdateEvent evt)
		{
			return Vector3.ZERO;
		}
	}
}

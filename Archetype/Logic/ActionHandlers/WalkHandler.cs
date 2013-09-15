using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Events;
using Archetype.Objects.Characters;
using Archetype.Utilities;

namespace Archetype.Logic.ActionHandlers
{
	public class WalkHandler : ActionHandler
	{
		public Vector3 Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
				Walking = true;
			}
		}
		public bool Walking { get; private set; }
		public float Velocity { get; private set; }

		private Vector3 _direction = Vector3.ZERO;

		public WalkHandler(Character actionPerformer, float velocity)
			: base(actionPerformer)
		{
			this.Velocity = velocity;
		}

		public void Stop()
		{
			Direction = Vector3.ZERO;
			Walking = false;
		}

		public override Vector3 GetVelocityInfluence(UpdateEvent evt)
		{
			if (!Walking || Direction == Vector3.ZERO)
				return Vector3.ZERO;

			Vector3 delta = Direction.TransformDelta(ActionPerformer.Position, Vector3.UNIT_SCALE, ActionPerformer.Orientation);
			return delta.Mask(true, false, true).NormalisedCopy * Velocity;
		}
	}
}

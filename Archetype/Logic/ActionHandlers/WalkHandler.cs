using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Objects.Characters;
using Archetype.Utilities;

namespace Archetype.Logic.ActionHandlers
{
	public class WalkHandler : ActionHandler
	{
		public float Velocity { get; private set; }

		private Vector3 _direction = Vector3.ZERO;

		public WalkHandler(Character actionPerformer, float velocity)
			: base(actionPerformer)
		{
			this.Velocity = velocity;
		}

		public void SetNextMoveDirection(Vector3 direction)
		{
			_direction = direction;
		}

		public override Vector3 GetVelocityInfluence(Events.UpdateEvent evt)
		{
			if (_direction == Vector3.ZERO)
				return Vector3.ZERO;

			Vector3 delta = _direction.TransformDelta(ActionPerformer.Position, Vector3.UNIT_SCALE, ActionPerformer.Orientation);
			_direction = Vector3.ZERO;
			return delta.Mask(true, false, true).NormalisedCopy * Velocity;
		}
	}
}

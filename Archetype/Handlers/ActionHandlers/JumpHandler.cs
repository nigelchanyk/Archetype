using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Objects.Characters;

namespace Archetype.Handlers.ActionHandlers
{
	public class JumpHandler : ActionHandler
	{
		public float GravityAcceleration { get; private set; }
		public float UpVelocity { get; private set; }

		private float _verticalVelocity = 0;

		public JumpHandler(Character actionPerformer, float upVelocity, float gravityAcceleration)
			: base(actionPerformer)
		{
			this.UpVelocity = upVelocity;
			this.GravityAcceleration = gravityAcceleration;
		}

		public virtual void Jump()
		{
			if (ActionPerformer.Position.y <= 0)
				_verticalVelocity = UpVelocity;
		}

		public override Vector3 GetVelocityInfluence(Events.UpdateEvent evt)
		{
			// On ground and velocity is downward
			if (ActionPerformer.Position.y <= 0 && _verticalVelocity < 0)
				return Vector3.ZERO;

			_verticalVelocity += GravityAcceleration * evt.ElapsedTime;
			return new Vector3(0, _verticalVelocity, 0);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Cameras;
using Archetype.Controllers.BotControllers.Strategies;
using Archetype.Events;
using Archetype.Objects;
using Archetype.UserInterface;
using Archetype.Utilities;

namespace Archetype.Controllers.BotControllers
{
	public class BotController : CameraController
	{
		// Tendency of pursuing escaped enemies
		public float AggressionFactor { get; private set; }
		public float Inaccuracy { get; private set; }
		public float ReactionDelay { get; private set; }

		protected Strategy Strategy { get; private set; }

		public BotController(World world, CameraManager cameraManager, Point windowCenter)
			: base(world, cameraManager, windowCenter, false)
		{
			this.Strategy = new EmptyStrategy(this);
		}

		public void AimTowards(UpdateEvent evt, Vector3 target)
		{
			float targetYaw = MathHelper.GetYaw(Character.Position, target);
		}

		public bool WalkTo(UpdateEvent evt, Vector3 target, float squaredDistanceThreshold)
		{
			if (Character.Position.SquaredDistance(target) < squaredDistanceThreshold)
				return true;

			float targetYaw = MathHelper.GetYaw(Character.Position, target);
			float angleDifference = MathHelper.AngleDifference(Character.Yaw, targetYaw);

			Character.Yaw = MathHelper.LerpAngle(Character.Yaw, targetYaw, evt.ElapsedTime * GameConstants.BotYawLerpAmount);

			// Only start walking if the angle is approximately in the target's general direction.
			if (angleDifference < MathHelper.PiOver6)
				Character.Walk(MathHelper.Forward);

			return false;
		}

		public override void Update(UpdateEvent evt)
		{
			base.Update(evt);
			Strategy = Strategy.NextStrategy();
			Strategy.Update(evt);
		}
	}
}

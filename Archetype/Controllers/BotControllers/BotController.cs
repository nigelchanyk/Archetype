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
using Archetype.Objects.Characters;

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
			Character.Yaw = MathHelper.LerpAngle(Character.Yaw, targetYaw, evt.ElapsedTime * GameConstants.BotAngleLerpAmount);
			float targetPitch = MathHelper.GetPitch(Character.EyePitch.ToPitchVector3(), target);
			Character.EyePitch = MathHelper.LerpAngle(Character.EyePitch, targetPitch, evt.ElapsedTime * GameConstants.BotAngleLerpAmount);
		}

		public Character GetVisibleEnemyBodyCollider(out BodyCollider collider)
		{
			foreach (Character enemy in World.BattleSystem.GetEnemiesAlive(Character))
			{
				collider = SelectBestCollider(enemy);
				if (collider != null)
					return enemy;
			}

			collider = null;
			return null;
		}

		public BodyCollider SelectBestCollider(Character enemy)
		{
			BodyCollider[] visibleColliders = enemy.GetFrustumCollisionResult(Character.ViewFrustum).ToArray();

			if (visibleColliders.Length == 0)
				return null;

			return visibleColliders.OrderByDescending(x => x.DamageMultiplier).First();
		}

		public bool WalkTo(UpdateEvent evt, Vector3 target, float squaredDistanceThreshold)
		{
			if (Character.Position.SquaredDistance(target) < squaredDistanceThreshold)
				return true;

			float targetYaw = MathHelper.GetYaw(Character.Position, target);
			float angleDifference = MathHelper.AngleDifference(Character.Yaw, targetYaw);

			Character.Yaw = MathHelper.LerpAngle(Character.Yaw, targetYaw, evt.ElapsedTime * GameConstants.BotAngleLerpAmount);

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

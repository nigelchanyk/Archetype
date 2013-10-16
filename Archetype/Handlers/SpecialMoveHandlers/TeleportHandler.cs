using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Events;
using Archetype.Objects.Characters;
using Archetype.Utilities;
using Archetype.Objects.Particles;

namespace Archetype.Handlers.SpecialMoveHandlers
{
	public class TeleportHandler : SpecialMoveHandler
	{
		private static readonly float TeleportDelay = 1;
		private static readonly float TeleportInterval = 3;

		private Vector3 Destination;
		private float TimeUntilTeleport = 0;

		public TeleportHandler(Character actionPerformer)
			: base(actionPerformer, TeleportInterval)
		{
		}

		protected override void OnTrigger()
		{
			Ray ray = ActionPerformer.GetEyeRay(MathHelper.Forward);
			float? intersection = ray.IntersectsPlane(new Plane(MathHelper.Up, 0));
			if (intersection == null)
				return;
			float? buildingIntersection = ActionPerformer.World.GetNearestIntersectingBuilding(ray);
			if (buildingIntersection.HasValue && buildingIntersection.Value < intersection.Value)
				return;

			TimeUntilTeleport = TeleportDelay;
			Destination = ray.GetPoint(intersection.Value);

			ActionPerformer.World.CreateParticleEmitterCluster(ParticleSystemType.Teleport, ActionPerformer.Position, false);
			ActionPerformer.World.CreateParticleEmitterCluster(ParticleSystemType.Teleport, Destination, false);
			ActionPerformer.LockMovement();
		}

		protected override void OnUpdate(UpdateEvent evt)
		{
			base.OnUpdate(evt);
			if (TimeUntilTeleport <= 0)
				return;
			TimeUntilTeleport -= evt.ElapsedTime;
			if (TimeUntilTeleport > 0)
				return;

			ActionPerformer.Position = Destination;
			ActionPerformer.UnlockMovement();
		}
	}
}

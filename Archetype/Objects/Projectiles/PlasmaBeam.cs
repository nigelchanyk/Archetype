using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Objects.Characters;
using Archetype.Events;
using Archetype.Objects.Particles;

namespace Archetype.Objects.Projectiles
{
	public class PlasmaBeam : Projectile
	{
		public int BaseDamage { get; private set; }

		private ParticleEmitterCluster Emitter;

		public PlasmaBeam(World world, Character creator, Vector3 position, Vector3 velocity, int baseDamage, float timeToLive)
			: base(world, creator, timeToLive)
		{
			this.Position = position;
			this.Velocity = velocity;
			this.BaseDamage = baseDamage;
			Emitter = World.CreateParticleEmitterCluster(ParticleSystemType.PlasmaBeam, position, true);
		}

		protected override void OnDispose()
		{
			Emitter.Stop();
		}

		protected override void OnUpdate(UpdateEvent evt)
		{
			Vector3 previousPosition = Position;
			base.OnUpdate(evt);
			if (!Alive)
				return;

			Emitter.Position = Position;

			// No ray created if not moved
			if (previousPosition == Position)
				return;

			Vector3 delta = Position - previousPosition;
			Ray ray = new Ray(previousPosition, delta);
			float? intersection;
			BodyCollider collider;
			Character enemy = World.FindEnemy(Creator, ray, out collider, out intersection);

			// No enemies intersect with ray. Check buildings.
			if (!intersection.HasValue)
				intersection = World.GetNearestIntersectingBuilding(ray);

			if (!intersection.HasValue || intersection.Value > delta.Length)
				return;
			Alive = false;
			if (enemy != null)
				enemy.ReceiveDamage(BaseDamage * collider.DamageMultiplier);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Events;
using Archetype.Objects.Characters;

namespace Archetype.Objects.Projectiles
{
	public abstract class Projectile : GeneralObject
	{
		public bool Alive { get; protected set; }
		public override Quaternion Orientation { get; set; }
		public override Vector3 Position { get; set; }
		public float RemainingTime { get; protected set; }

		protected Character Creator { get; private set; }

		public Projectile(World world, Character creator, float timeToLive)
			: base(world)
		{
			this.Creator = creator;
			this.RemainingTime = timeToLive;
			Alive = true;
		}

		protected override void OnUpdate(UpdateEvent evt)
		{
			base.OnUpdate(evt);
			Position += Velocity * evt.ElapsedTime;
			RemainingTime -= evt.ElapsedTime;

			if (RemainingTime <= 0)
				Alive = false;
		}
	}
}

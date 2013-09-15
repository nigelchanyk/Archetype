using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Controllers.BotControllers.Strategies;
using Archetype.Events;
using Archetype.Objects;
using Archetype.UserInterface;

namespace Archetype.Controllers.BotControllers
{
	public abstract class BotController : CameraController
	{
		// Tendency of pursuing escaped enemies
		public float AggressionFactor { get; private set; }
		public float Inaccuracy { get; private set; }
		public float ReactionDelay { get; private set; }

		protected Strategy Strategy { get; private set; }

		public BotController(World world, Point windowCenter)
			: base(world, windowCenter, false)
		{
			this.Strategy = new RoamStrategy(this);
		}

		public override void Update(UpdateEvent evt)
		{
			base.Update(evt);
		}
	}
}

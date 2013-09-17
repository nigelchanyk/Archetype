using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Events;
using Mogre;

namespace Archetype.Controllers.BotControllers.Strategies
{
	public class RoamStrategy : Strategy
	{
		public Vector3 Destination { get; private set; }
		public Vector3 Direction { get; private set; }

		public RoamStrategy(BotController botController)
			: base(botController)
		{
			Vector3? destination = BotController.Character.World.GetClosestVertex(BotController.Character.Position);
			if (!destination.HasValue)
				throw new InvalidOperationException("Unable to find direct vertex for roaming.");
			this.Destination = destination.Value;
			Direction = (this.Destination - BotController.Character.Position).NormalisedCopy;
		}

		public override Strategy NextStrategy()
		{
			if (BotController.Character == null)
				return new RoamStrategy(BotController);

			return this;
		}

		public override void Update(UpdateEvent evt)
		{
		}
	}
}

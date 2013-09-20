using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Events;
using Archetype.Utilities;
using Archetype.Objects.Characters;

namespace Archetype.Controllers.BotControllers.Strategies
{
	public class RoamStrategy : Strategy
	{
		private static readonly float SquaredDistanceThreshold = 0.4f * 0.4f;

		public Vector3 Destination { get; private set; }
		public Vector3 PreviousDestination { get; private set; }

		public RoamStrategy(BotController botController)
			: base(botController)
		{
			Vector3? destination = BotController.Character.World.GetClosestVertex(BotController.Character.Position);
			if (!destination.HasValue)
				throw new InvalidOperationException("Unable to find direct vertex for roaming.");
			this.Destination = destination.Value;
			PreviousDestination = BotController.Character.Position;
		}

		public override Strategy NextStrategy()
		{
			Strategy strategy = base.NextStrategy();
			if (strategy != this)
				return strategy;

			BodyCollider visibleCollider;
			Character enemy = BotController.GetVisibleEnemyBodyCollider(out visibleCollider);
			if (enemy != null)
				return new ReactionDelayStrategy(BotController, enemy, visibleCollider);

			return this;
		}

		public override void Update(UpdateEvent evt)
		{
			bool approached = BotController.WalkTo(evt, Destination, SquaredDistanceThreshold);
			if (!approached)
				return;

			Vector3[] possibleNextNodes = BotController.Character.World.GetAdjacentVertices(Destination);
			// Avoid walking back to where we come from if possible.
			if (possibleNextNodes.Length == 1)
				Destination = possibleNextNodes[0];
			else
			{
				possibleNextNodes.Shuffle();
				Vector3 next = possibleNextNodes.First(x => !x.IsApproximately(PreviousDestination, 0.0001f));
				PreviousDestination = Destination;
				Destination = next;
			}
		}
	}
}

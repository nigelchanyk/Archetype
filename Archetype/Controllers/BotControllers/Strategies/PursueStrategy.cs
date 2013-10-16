using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Objects.Characters;
using Archetype.Logic;
using Archetype.Utilities;
using Archetype.Events;

namespace Archetype.Controllers.BotControllers.Strategies
{
	public class PursueStrategy : Strategy
	{
		private PathNode Target = null;

		public PursueStrategy(BotController controller, Vector3 destination)
			: base(controller)
		{
			Path path = BotController.Character.World.FindPath(BotController.Character.Position, destination);
			if (path != null && path.NodeCount > 0)
				Target = path.PathNodes[0];
		}

		public override Strategy NextStrategy()
		{
			Strategy next = base.NextStrategy();
			if (next != this)
				return next;

			BodyCollider visibleCollider;
			Character enemy = BotController.GetVisibleEnemyBodyCollider(out visibleCollider);
			if (enemy != null)
				return new ReactionDelayStrategy(BotController, enemy, visibleCollider);
			if (Target == null)
				return new RoamStrategy(BotController);

			return this;
		}

		public override void Update(UpdateEvent evt)
		{
			bool approached = BotController.WalkTo(evt, Target.Position);
			if (!approached)
				return;

			Target = Target.Next;
		}
	}
}

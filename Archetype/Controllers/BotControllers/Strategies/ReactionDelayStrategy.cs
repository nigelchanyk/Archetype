using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Events;
using Archetype.Objects.Characters;

namespace Archetype.Controllers.BotControllers.Strategies
{
	public class ReactionDelayStrategy : EnemyInteractionStrategy
	{
		private float ElapsedDelay = 0;

		public ReactionDelayStrategy(BotController controller, Character enemy, BodyCollider visibleCollider)
			: base(controller, enemy, visibleCollider)
		{
		}

		public override Strategy NextStrategy()
		{
			if (BotController.Character == null)
				return new EmptyStrategy(BotController);
			if (!Enemy.Alive)
				return new RoamStrategy(BotController);
			if (!IsEnemyVisible())
				return new RoamStrategy(BotController);

			return this;
		}

		public override void Update(UpdateEvent evt)
		{
			ElapsedDelay += evt.ElapsedTime;

			BotController.Character.Stop();
		}
	}
}

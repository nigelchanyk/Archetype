using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Events;
using Archetype.Objects.Characters;

namespace Archetype.Controllers.BotControllers.Strategies
{
	public class AttackStrategy : EnemyInteractionStrategy
	{
		public AttackStrategy(BotController controller, Character enemy, BodyCollider visibleCollider)
			: base(controller, enemy, visibleCollider)
		{
		}

		public override Strategy NextStrategy()
		{
			Strategy strategy = base.NextStrategy();
			if (strategy != this)
				return strategy;

			if (!IsEnemyVisible())
				return new RoamStrategy(BotController);

			return this;
		}

		public override void Update(UpdateEvent evt)
		{
			BotController.Character.Stop();
			BotController.AimTowards(evt, VisibleCollider.PrimitiveNode.GetCenter(true));
			BotController.Character.RegularAttack();
		}
	}
}

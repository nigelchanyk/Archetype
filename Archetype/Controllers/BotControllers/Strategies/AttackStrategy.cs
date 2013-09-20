using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype.Controllers.BotControllers.Strategies
{
	public class AttackStrategy : Strategy
	{
		public AttackStrategy(BotController controller)
			: base(controller)
		{
		}

		public override Strategy NextStrategy()
		{
			if (BotController.Character == null)
				return new EmptyStrategy(BotController);

			return this;
		}

		public override void Update(Events.UpdateEvent evt)
		{
		}
	}
}

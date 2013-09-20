using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Events;

namespace Archetype.Controllers.BotControllers.Strategies
{
	public abstract class Strategy
	{
		public BotController BotController { get; private set; }

		public Strategy(BotController botController)
		{
			this.BotController = botController;
		}

		public virtual Strategy NextStrategy()
		{
			if (BotController.Character == null)
				return new EmptyStrategy(BotController);

			return this;
		}

		public abstract void Update(UpdateEvent evt);
	}
}

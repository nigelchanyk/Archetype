using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Events;
using Archetype.Objects.Characters;

namespace Archetype.Controllers.BotControllers.Strategies
{
	/// <summary>
	/// An empty strategy for BotController without a character.
	/// </summary>
	public class EmptyStrategy : Strategy
	{
		public EmptyStrategy(BotController botController)
			: base(botController)
		{
		}

		public override Strategy NextStrategy()
		{
			if (BotController.Character != null)
				return new RoamStrategy(BotController);

			return this;
		}

		public override void Update(UpdateEvent evt)
		{
			BotController.Character.Stop();
		}
	}
}

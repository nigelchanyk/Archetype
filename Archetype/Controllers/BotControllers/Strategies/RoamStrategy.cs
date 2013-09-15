using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Events;

namespace Archetype.Controllers.BotControllers.Strategies
{
	public class RoamStrategy : Strategy
	{
		public RoamStrategy(BotController botController)
			: base(botController)
		{
		}

		public override void Update(UpdateEvent evt)
		{
		}
	}
}

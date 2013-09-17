using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.BattleSystems;
using Archetype.Events;
using Archetype.Objects;
using Archetype.UserInterface;

namespace Archetype.Controllers.BotControllers
{
	public class BotManager
	{
		public BattleSystem BattleSystem { get; private set; }

		private List<BotController> Controllers = new List<BotController>();

		public BotManager(World world, BattleSystem battleSystem, string[] playerNames, Point windowCenter)
		{
			this.BattleSystem = battleSystem;
			foreach (BattlerRecord record in BattleSystem.GetAllRecords())
			{
				// Player names are assumed to be small.
				// Therefore, no optimization is performed.
				if (playerNames.Contains(record.Name))
					continue;

				BotController controller = new BotController(world, windowCenter);
				controller.Character = record.Character;
				Controllers.Add(controller);
			}
		}

		public void Update(UpdateEvent evt)
		{
			Controllers.ForEach(x => x.Update(evt));
		}
	}
}

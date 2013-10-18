using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.BattleSystems;
using Archetype.Events;
using Archetype.Objects;
using Archetype.UserInterface;
using Archetype.Cameras;

namespace Archetype.Controllers.BotControllers
{
	public class BotManager
	{
		public BattleSystem BattleSystem { get; private set; }

		private CameraManager CameraManager;
		private List<BotController> Controllers = new List<BotController>();
		private Point WindowCenter;
		private string[] PlayerNames;
		private World World;

		public BotManager(World world, CameraManager cameraManager, BattleSystem battleSystem, string[] playerNames, Point windowCenter)
		{
			this.World = world;
			this.CameraManager = cameraManager;
			this.BattleSystem = battleSystem;
			this.PlayerNames = playerNames;
			this.WindowCenter = windowCenter;
			Reset();
		}

		public void Reset()
		{
			Controllers.Clear();
			foreach (BattlerRecord record in BattleSystem.GetAllRecords())
			{
				// Player names are assumed to be small.
				// Therefore, no optimization is performed.
				if (PlayerNames.Contains(record.Name))
					continue;

				BotController controller = new BotController(World, CameraManager, WindowCenter);
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

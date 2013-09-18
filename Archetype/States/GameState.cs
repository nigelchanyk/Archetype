using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Applications;
using Archetype.BattleSystems;
using Archetype.Controllers;
using Archetype.Events;
using Archetype.UserInterface.Crosshairs;
using Archetype.Controllers.BotControllers;

namespace Archetype.States
{
	public class GameState : WorldState
	{
		private BattleSystem BattleSystem;
		private BotManager BotManager;
		private Crosshair Crosshair;
		private Player Player;

		public GameState(Application application, BattleSystem battleSystem, string playerName)
			: base(application, "test_scene")
		{
			this.BattleSystem = battleSystem;
			BattleSystem.World = World;
			World.BattleSystem = BattleSystem;
			BattleSystem.Start();

			Player = new Player(World, CameraManager, Application.WindowCenter, true);
			Player.Character = BattleSystem.GetCharacterByName(playerName);
			BotManager = new BotManager(World, CameraManager, BattleSystem, new string[] { playerName }, Application.WindowCenter);

			Crosshair = new Crosshair(Application.Resolution, Player.Camera)
			{
				Character = Player.Character
			};
			UserInterface.Add(Crosshair);
			CursorVisible = false;
		}

		protected override void OnDispose()
		{
			base.OnDispose();
		}

		protected override void OnKeyPress(MOIS.KeyEvent evt)
		{
			if (evt.key == MOIS.KeyCode.KC_ESCAPE)
				Application.Exit = true;
		}

		protected override void OnResume(UpdateEvent evt)
		{
			base.OnResume(evt);
		}

		protected override void OnPause(UpdateEvent evt)
		{
			base.OnPause(evt);
		}

		protected override void OnUpdate(UpdateEvent evt)
		{
			base.OnUpdate(evt);
			Player.Update(evt);
			BotManager.Update(evt);
			Application.CenterCursor();
		}
	}
}

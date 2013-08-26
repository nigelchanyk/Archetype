using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

using Archetype.Applications;
using Archetype.Controllers;
using Archetype.Events;
using Archetype.BattleSystems;

namespace Archetype.States
{
	public class GameState : WorldState
	{
		private BattleSystem BattleSystem;
		private Player Player;

		public GameState(Application application, BattleSystem battleSystem, string playerName)
			: base(application, "test_scene")
		{
			this.BattleSystem = battleSystem;
			BattleSystem.World = World;
			World.BattleSystem = BattleSystem;
			Player = new Player(World, true);
			BattleSystem.Start();
			Player.Character = BattleSystem.GetCharacterByName(playerName);
		}

		protected override void OnDispose()
		{
			base.OnDispose();
		}

		protected override void OnKeyPressed(MOIS.KeyEvent evt)
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
		}
	}
}

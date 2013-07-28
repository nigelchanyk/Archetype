using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

using Archetype.Applications;
using Archetype.Controllers;
using Archetype.Events;

namespace Archetype.States
{
	public class GameState : WorldState
	{
		private Player _player;

		public GameState(Application application)
			: base(application)
		{
			_player = new Player(World, true);
			
		}

		protected override void OnDispose()
		{
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
			_player.Update(evt);
		}
	}
}

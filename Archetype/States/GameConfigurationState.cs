using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Applications;
using Archetype.BattleSystems;
using Archetype.Events;

namespace Archetype.States
{
	public class GameConfigurationState : State
	{

		public GameConfigurationState(Application application)
			: base(application)
		{
		}

		protected override void OnDispose()
		{
			base.OnDispose();
		}

		protected override void OnPause(UpdateEvent evt)
		{
			base.OnPause(evt);
		}

		protected override void OnResume(UpdateEvent evt)
		{
			base.OnResume(evt);
		}

		protected override void OnUpdate(UpdateEvent evt)
		{
		}
	}
}

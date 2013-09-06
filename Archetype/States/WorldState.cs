using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Applications;
using Archetype.Objects;

namespace Archetype.States
{
	public abstract class WorldState : State
	{
		public World World { get; private set; }

		private bool AddedToViewport = false;

		public WorldState(Application application, string scene = "")
			: base(application)
		{
			World = new World(Application.Root, scene);
		}

		protected override void OnDispose()
		{
			Application.Window.RemoveViewport(ZOrder);
			World.Dispose();
		}

		protected override void OnPause(Events.UpdateEvent evt)
		{
			if (!DrawWhenInactive && AddedToViewport)
			{
				Application.Window.RemoveViewport(ZOrder);
				AddedToViewport = false;
			}
		}

		protected override void OnResume(Events.UpdateEvent evt)
		{
			if (!AddedToViewport)
			{
				Application.Window.AddViewport(World.Camera, ZOrder);
			}
		}

		protected override void OnUpdate(Events.UpdateEvent evt)
		{
			UserInterface.Update(evt);
			World.Update(evt);
		}
	}
}

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

		private bool _addedToViewport = false;

		public WorldState(Application application)
			: base(application)
		{
			World = new World(Application.Root, "test_scene.scene");
		}

		protected override void OnDispose()
		{
			World.Dispose();
		}

		protected override void OnPause(Events.UpdateEvent evt)
		{
			if (!DrawWhenInactive && _addedToViewport)
			{
				Application.Window.RemoveViewport(ZOrder);
				_addedToViewport = false;
			}
		}

		protected override void OnResume(Events.UpdateEvent evt)
		{
			if (!_addedToViewport)
			{
				Application.Window.AddViewport(World.Camera, ZOrder);
			}
		}

		protected override void OnUpdate(Events.UpdateEvent evt)
		{
			World.Update(evt);
		}
	}
}

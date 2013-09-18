using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Applications;
using Archetype.Cameras;
using Archetype.Objects;

namespace Archetype.States
{
	public abstract class WorldState : State
	{
		public CameraManager CameraManager { get; private set; }
		public World World { get; private set; }

		private bool AddedToViewport = false;

		public WorldState(Application application, string scene = "")
			: base(application)
		{
			World = new World(Application.Root, scene);
			CameraManager = new CameraManager();
			CameraManager.CameraChanged += OnActiveCameraChanged;
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
				World.Paused = true;
			}
		}

		protected override void OnResume(Events.UpdateEvent evt)
		{
			if (!AddedToViewport)
			{
				if (CameraManager.ActiveCamera == null)
					throw new InvalidOperationException("There should always be an active camera.");

				Application.Window.AddViewport(CameraManager.ActiveCamera, ZOrder);
				World.Paused = false;
			}
		}

		protected override void OnUpdate(Events.UpdateEvent evt)
		{
			UserInterface.Update(evt);
			World.Update(evt);
		}

		private void OnActiveCameraChanged(object sender, EventArgs args)
		{
			if (AddedToViewport)
			{
				// Replace existing viewport
				Application.Window.RemoveViewport(ZOrder);
				Application.Window.AddViewport(CameraManager.ActiveCamera, ZOrder);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Events;
using Archetype.States;

namespace Archetype.Objects
{
	public abstract class GeneralObject : IDisposable
	{
		public abstract Quaternion Orientation { get; set; }
		public GeneralObject Parent { get; private set; }
		public abstract Vector3 Position { get; set; }
		public Vector3 Velocity { get; set; }

		public World World { get; private set; }

		public GeneralObject(World world)
		{
			this.World = world;
		}

		public void Dispose()
		{
			OnDispose();
		}

		public void KeyPressed(MOIS.KeyEvent evt)
		{
			OnKeyPressed(evt);
		}

		public void Resume(UpdateEvent evt)
		{
			OnResume(evt);
		}

		public void Pause(UpdateEvent evt)
		{
			OnPause(evt);
		}

		public void Update(UpdateEvent evt)
		{
			OnUpdate(evt);
		}

		protected virtual void OnDispose() {}
		protected virtual void OnKeyPressed(MOIS.KeyEvent evt) {}
		protected virtual void OnResume(UpdateEvent evt) {}
		protected virtual void OnPause(UpdateEvent evt) {}
		protected virtual void OnUpdate(UpdateEvent evt) {}
	}
}

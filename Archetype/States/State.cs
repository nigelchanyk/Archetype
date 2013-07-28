using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

using Archetype.Applications;
using Archetype.Events;
using Archetype.Objects;

namespace Archetype.States
{
	public abstract class State : IDisposable
	{
		public Application Application { get; private set; }
		public bool DrawWhenInactive { get; protected set; }
		public int ZOrder { get; set; }

		public State(Application application)
		{
			Application = application;
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
		protected abstract void OnUpdate(UpdateEvent evt);
	}
}

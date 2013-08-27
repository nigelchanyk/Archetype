using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

using Archetype.Applications;
using Archetype.Events;
using Archetype.Objects;
using Archetype.UserInterface;

namespace Archetype.States
{
	public abstract class State : IDisposable
	{
		public Application Application { get; private set; }
		public bool DrawWhenInactive { get; protected set; }
		public ushort ZOrder
		{
			get
			{
				return _zOrder;
			}
			set
			{
				_zOrder = value;
				if (UserInterface != null)
					UserInterface.ZOrder = value;
			}
		}

		protected UserInterfaceLayer UserInterface { get; private set; }

		private bool IsUserInterfaceInitialized = false;
		private ushort _zOrder;

		public State(Application application)
		{
			Application = application;
		}

		public void Dispose()
		{
			OnDispose();
			DisposeUserInterface();
		}

		public void KeyPress(MOIS.KeyEvent evt)
		{
			OnKeyPress(evt);
		}

		public void MouseMove(MOIS.MouseEvent evt)
		{
			OnMouseMove(evt);
		}

		public void MousePress(MOIS.MouseEvent evt, MOIS.MouseButtonID id)
		{
			OnMousePress(evt, id);
		}

		public void MouseRelease(MOIS.MouseEvent evt, MOIS.MouseButtonID id)
		{
			OnMouseRelease(evt, id);
		}

		public void Resume(UpdateEvent evt)
		{
			OnResume(evt);
			UserInterface = new UserInterfaceLayer(ZOrder);
			OnUserInterfaceCreate();
			IsUserInterfaceInitialized = true;
		}

		public void Pause(UpdateEvent evt)
		{
			OnPause(evt);
			DisposeUserInterface();
		}

		public void Update(UpdateEvent evt)
		{
			OnUpdate(evt);
		}

		protected virtual void OnDispose() {}
		protected virtual void OnKeyPress(MOIS.KeyEvent evt) {}
		protected virtual void OnMouseMove(MOIS.MouseEvent evt) {}
		protected virtual void OnMousePress(MOIS.MouseEvent evt, MOIS.MouseButtonID id) {}
		protected virtual void OnMouseRelease(MOIS.MouseEvent evt, MOIS.MouseButtonID id) {}
		protected virtual void OnResume(UpdateEvent evt) {}
		protected virtual void OnPause(UpdateEvent evt) {}
		protected virtual void OnUserInterfaceCreate() {}
		protected virtual void OnUserInterfaceDispose() {}
		protected abstract void OnUpdate(UpdateEvent evt);

		private void DisposeUserInterface()
		{
			if (IsUserInterfaceInitialized)
			{
				OnUserInterfaceDispose();
				UserInterface.Dispose();
				IsUserInterfaceInitialized = false;
			}
		}
	}
}

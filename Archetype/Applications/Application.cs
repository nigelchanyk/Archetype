using MOIS;
using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.States;
using Archetype.Events;

namespace Archetype.Applications
{
	public abstract class Application
	{
		public MOIS.InputManager Input { get; private set; }
		public MOIS.Keyboard Keyboard { get; private set; }
		public MOIS.Mouse Mouse { get; private set; }
		public Root Root { get; private set; }
		public string Title { get; set; }
		public RenderWindow Window { get; private set; }

		public bool Exit { get; set; }

		private State _newStateScheduled; // Nullable
		private Stack<State> _stateStack = new Stack<State>();
		private int _popStateCount = 0;

		public Application(string title)
		{
			Root = new Root();
			ImportResources();
			Title = title;
		}

		public void SchedulePopState()
		{
			_popStateCount++;
		}

		public void SchedulePushState(State newState)
		{
			if (_newStateScheduled != null)
				throw new Exception("New state is already scheduled.");
			_newStateScheduled = newState;
		}

		public void ShowApplication()
		{
			Window = Root.Initialise(true, Title);
			TextureManager.Singleton.DefaultNumMipmaps = 5;
			ResourceGroupManager.Singleton.InitialiseAllResourceGroups();
			InitializeInput();

			Initialize();
			Root.FrameRenderingQueued += new FrameListener.FrameRenderingQueuedHandler(OnFrameRenderingQueued);
			Root.StartRendering();
		}

		public bool ShowDisplayConfiguration()
		{
			return Root.ShowConfigDialog();
		}

		protected virtual void Initialize()
		{
		}

		private void ImportResources()
		{
			ConfigFile config = new ConfigFile();
			config.Load("resources.cfg", "=", true);

			ConfigFile.SectionIterator itr = config.GetSectionIterator();
			while (itr.MoveNext())
			{
				foreach (var line in itr.Current)
					ResourceGroupManager.Singleton.AddResourceLocation(line.Value, line.Key, itr.CurrentKey);
			}
		}

		private void InitializeInput()
		{
			int handle;
			Window.GetCustomAttribute("WINDOW", out handle);
			Input = MOIS.InputManager.CreateInputSystem((uint)handle);
			Keyboard = (MOIS.Keyboard)Input.CreateInputObject(MOIS.Type.OISKeyboard, true);
			Mouse = (MOIS.Mouse)Input.CreateInputObject(MOIS.Type.OISMouse, true);
			Keyboard.KeyPressed += new MOIS.KeyListener.KeyPressedHandler(OnKeyPressed);
		}

		private void ManageScheduledStateEvent(UpdateEvent evt)
		{
			while (_popStateCount > 0)
			{
				State removed = _stateStack.Pop();
				removed.Pause(evt);
				_popStateCount--;
			}
			if (_newStateScheduled != null)
			{
				_stateStack.Push(_newStateScheduled);
				_newStateScheduled.ZOrder = _stateStack.Count;
				_newStateScheduled.Resume(evt);
				_newStateScheduled = null;
			}
		}

		private bool OnFrameRenderingQueued(FrameEvent evt)
		{
			// Why am I doing extra work when elapsed time is 0?
			if (evt.timeSinceLastFrame == 0)
				return !Exit;

			Keyboard.Capture();
			Mouse.Capture();
			Update(new UpdateEvent(Keyboard, Mouse, evt));

			return !Exit;
		}

		private bool OnKeyPressed(KeyEvent evt)
		{
			if (_stateStack.Count > 0)
				_stateStack.Peek().KeyPressed(evt);
			return true;
		}

		private void Update(UpdateEvent evt)
		{
			ManageScheduledStateEvent(evt);
			if (_stateStack.Count > 0)
				_stateStack.Peek().Update(evt);
		}

	}
}

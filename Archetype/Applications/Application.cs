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

		private State NewStateScheduled; // Nullable
		private Stack<State> StateStack = new Stack<State>();
		private int PopStateCount = 0;

		public Application(string title)
		{
			Root = new Root();
			ImportResources();
			Title = title;
		}

		public void SchedulePopState()
		{
			PopStateCount++;
		}

		public void SchedulePushState(State newState)
		{
			if (NewStateScheduled != null)
				throw new Exception("New state is already scheduled.");
			NewStateScheduled = newState;
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
			while (PopStateCount > 0)
			{
				State removed = StateStack.Pop();
				removed.Pause(evt);
				PopStateCount--;
			}
			if (NewStateScheduled != null)
			{
				StateStack.Push(NewStateScheduled);
				NewStateScheduled.ZOrder = StateStack.Count;
				NewStateScheduled.Resume(evt);
				NewStateScheduled = null;
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
			if (StateStack.Count > 0)
				StateStack.Peek().KeyPressed(evt);
			return true;
		}

		private void Update(UpdateEvent evt)
		{
			ManageScheduledStateEvent(evt);
			if (StateStack.Count > 0)
				StateStack.Peek().Update(evt);
		}

	}
}

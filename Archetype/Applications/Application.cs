using MOIS;
using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.States;
using Archetype.Events;
using Archetype.UserInterface;

namespace Archetype.Applications
{
	public abstract class Application : IDisposable
	{
		public bool CursorVisible
		{
			get { return CursorOverlay.Visible; }
			set { CursorOverlay.Visible = value; }
		}
		public MOIS.InputManager Input { get; private set; }
		public MOIS.Keyboard Keyboard { get; private set; }
		public MOIS.Mouse Mouse { get; private set; }
		public Root Root { get; private set; }
		public string Title { get; set; }
		public RenderWindow Window { get; private set; }
		public Point WindowCenter { get; private set; }

		public bool Exit { get; set; }

		private CursorOverlay CursorOverlay;
		private State NewStateScheduled; // Nullable
		private Stack<State> StateStack = new Stack<State>();
		private int PopStateCount = 0;
		private WindowExitListener ExitListener = new WindowExitListener();

		public Application(string title)
		{
			Root = new Root();
			ImportResources();
			Title = title;
		}

		public void CenterCursor()
		{
			var x = Mouse.MouseState.X;
			var y = Mouse.MouseState.Y;
			x.abs = WindowCenter.X;
			y.abs = WindowCenter.Y;
		}

		public void Dispose()
		{
			while (StateStack.Count > 0)
				StateStack.Pop().Dispose();
			ExitListener.Dispose();
			Input.Dispose();
			Keyboard.Dispose();
			Mouse.Dispose();
			Window.Dispose();
			Root.Dispose();
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
			WindowCenter = new Point((int)Root.AutoCreatedWindow.Width / 2, (int)Root.AutoCreatedWindow.Height / 2);
			TextureManager.Singleton.DefaultNumMipmaps = 5;
			ResourceGroupManager.Singleton.InitialiseAllResourceGroups();
			InitializeInput();
			if (CursorOverlay == null)
				CursorOverlay = new CursorOverlay(GameConstants.CursorZOrder);
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
			WindowEventUtilities.AddWindowEventListener(Window, ExitListener);
			ExitListener.WindowClose += new EventHandler(OnWindowClosed);
			Input = MOIS.InputManager.CreateInputSystem((uint)handle);
			Keyboard = (MOIS.Keyboard)Input.CreateInputObject(MOIS.Type.OISKeyboard, true);
			Mouse = (MOIS.Mouse)Input.CreateInputObject(MOIS.Type.OISMouse, true);
			Keyboard.KeyPressed += new MOIS.KeyListener.KeyPressedHandler(OnKeyPress);
			Mouse.MouseMoved += new MOIS.MouseListener.MouseMovedHandler(OnMouseMove);
			Mouse.MousePressed += new MOIS.MouseListener.MousePressedHandler(OnMousePress);
			Mouse.MouseReleased += new MOIS.MouseListener.MouseReleasedHandler(OnMouseRelease);
			MOIS.MouseState_NativePtr mouseState = Mouse.MouseState;
			mouseState.width = (int)Root.AutoCreatedWindow.Width;
			mouseState.height = (int)Root.AutoCreatedWindow.Height;
		}

		private void ManageScheduledStateEvent(UpdateEvent evt)
		{
			if (PopStateCount > 0)
			{
				while (PopStateCount > 0)
				{
					StateStack.Pop().Dispose();
					PopStateCount--;
				}
				if (StateStack.Count > 0)
					StateStack.Peek().Resume(evt);

			}
			if (NewStateScheduled != null)
			{
				if (StateStack.Count > 0)
					StateStack.Peek().Pause(evt);
				StateStack.Push(NewStateScheduled);
				NewStateScheduled.ZOrder = (ushort)StateStack.Count;
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

		private bool OnKeyPress(KeyEvent evt)
		{
			if (StateStack.Count > 0)
				StateStack.Peek().KeyPress(evt);
			return true;
		}

		private bool OnMouseMove(MouseEvent evt)
		{
			if (CursorOverlay != null)
				CursorOverlay.MouseMove(evt);
			if (StateStack.Count > 0)
				StateStack.Peek().MouseMove(evt);
			return true;
		}

		private bool OnMousePress(MouseEvent evt, MouseButtonID id)
		{
			if (StateStack.Count > 0)
				StateStack.Peek().MousePress(evt, id);
			return true;
		}

		private bool OnMouseRelease(MouseEvent evt, MouseButtonID id)
		{
			if (StateStack.Count > 0)
				StateStack.Peek().MouseRelease(evt, id);
			return true;
		}

		private void OnWindowClosed(object sender, EventArgs e)
		{
			Exit = true;
		}

		private void Update(UpdateEvent evt)
		{
			ManageScheduledStateEvent(evt);
			if (StateStack.Count > 0)
				StateStack.Peek().Update(evt);
		}

	}
}

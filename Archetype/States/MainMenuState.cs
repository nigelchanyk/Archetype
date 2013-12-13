using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Applications;
using Archetype.UserInterface;

namespace Archetype.States
{
	public class MainMenuState : WorldState
	{
		private Camera Camera;
		private Menu Menu;

		public MainMenuState(Application application)
			: base(application)
		{
			CursorVisible = true;
			Camera = World.CreateCamera(new Vector3(0, 0, -5), Vector3.ZERO);
			World.WorldNode.AttachObject(Camera);
			CameraManager.ActiveCamera = Camera;
		}

		protected override void OnDispose()
		{
			Camera.Dispose();
			base.OnDispose();
		}

		protected override void OnUserInterfaceCreate()
		{
			base.OnUserInterfaceCreate();
			FontManager.Singleton.GetByName("BlueHighway").Load();
			KeyValuePair<string, EventHandler>[] menuEvents =
			{
				new KeyValuePair<string, EventHandler>("Start Game", new EventHandler(OnStartGame)),
				new KeyValuePair<string, EventHandler>("Exit", new EventHandler(OnExit))
			};
			Menu = new Menu(new Point(100, 300), menuEvents.Length);
			foreach (var menuEvent in menuEvents)
				Menu.Add(menuEvent.Key, menuEvent.Value);
			UserInterface.Add(Menu);
		}

		protected override void OnUserInterfaceDispose()
		{
			base.OnUserInterfaceDispose();
			Menu.Dispose();
		}

		private void OnStartGame(object sender, EventArgs e)
		{
			Application.SchedulePopState();
			Application.SchedulePushState(new GameConfigurationState(Application));
		}

		private void OnExit(object sender, EventArgs e)
		{
			Application.Exit = true;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Applications;
using Archetype.BattleSystems;
using Archetype.Events;
using Archetype.UserInterface;

namespace Archetype.States
{
	public class GameConfigurationState : WorldState
	{
		private Camera Camera;
		private Button StartButton;

		public GameConfigurationState(Application application)
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

		protected override void OnPause(UpdateEvent evt)
		{
			base.OnPause(evt);
		}

		protected override void OnResume(UpdateEvent evt)
		{
			base.OnResume(evt);
		}

		protected override void OnUpdate(UpdateEvent evt)
		{
		}

		protected override void OnUserInterfaceCreate()
		{
			base.OnUserInterfaceCreate();
			FontManager.Singleton.GetByName("BlueHighway").Load();
			StartButton = new Button(
				"Start",
				new Style
				{
					Dimension = new Dimension(200, 70),
					Position = new Point(400, 300),
					Material = "Core/StatsBlockCenter"
				},
				new Style
				{
					Dimension = new Dimension(150, 50),
					Position = new Point(20, 20),
					FontSize = 20,
					Color = ColourValue.White,
					Font = "BlueHighway"
				}
			);
			StartButton.Clicked += new EventHandler(OnStartClick);
			UserInterface.Add(StartButton);
		}

		protected override void OnUserInterfaceDispose()
		{
			StartButton.Dispose();
		}

		private void OnStartClick(object sender, EventArgs e)
		{
			TeamBattle battle = new TeamBattle();
			battle.AddPlayer("Test", TeamBattle.Team.Red);
			battle.AddBots(0, 1);
			Application.SchedulePopState();
			Application.SchedulePushState(new GameState(Application, battle, "Test"));
		}
	}
}

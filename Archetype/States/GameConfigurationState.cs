using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Applications;
using Archetype.BattleSystems;
using Archetype.Events;
using Archetype.UserInterface;
using Mogre;

namespace Archetype.States
{
	public class GameConfigurationState : WorldState
	{
		private Button StartButton;

		public GameConfigurationState(Application application)
			: base(application)
		{
		}

		protected override void OnDispose()
		{
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
					Material = "Core/StatsBlockCenter"
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
			battle.AddBots(3, 4);
			Application.SchedulePopState();
			Application.SchedulePushState(new GameConfigurationState(Application));
		}
	}
}

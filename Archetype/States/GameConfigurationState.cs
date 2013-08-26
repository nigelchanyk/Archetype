using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Applications;
using Archetype.BattleSystems;
using Archetype.Events;
using Archetype.UserInterface;
using Miyagi.UI.Controls;
using Miyagi.Common.Data;
using Mogre;
using Miyagi.Backend.Mogre;

namespace Archetype.States
{
	public class GameConfigurationState : WorldState
	{
		private Button StartButton;
		private UserInterfaceLayer UserInterface;

		public GameConfigurationState(Application application)
			: base(application)
		{
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			Application.DestroyUserInterfaceLayer(UserInterface);
			StartButton.Dispose();
		}

		protected override void OnPause(UpdateEvent evt)
		{
			base.OnPause(evt);
		}

		protected override void OnResume(UpdateEvent evt)
		{
			base.OnResume(evt);
			UserInterface = Application.CreateUserInterfaceLayer();
			StartButton = new Button
			{
				Text = "Start",
				TextStyle = new Miyagi.UI.Controls.Styles.TextStyle { Font = UserInterface.FontCollection["BlueHighwayImage"] },
				Size = new Size(200, 50),
				Location = new Point(300, 300),
				Skin = UserInterface.SkinCollection["ButtonSkin"]
			};
			UserInterface.Controls.Add(StartButton);
			UserInterface.SetCursor("CursorSkin");
		}

		protected override void OnUpdate(UpdateEvent evt)
		{
		}
	}
}

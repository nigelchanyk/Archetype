using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;
using Archetype.Events;

namespace Archetype.UserInterface.Game
{
	public class Notification : UserInterfaceComponent, IDisposable
	{
		public Dimension Resolution { get; private set; }

		private Action Callback;
		private float RemainingTime = 0;
		private PanelOverlayElement Panel;
		private TextAreaOverlayElement TextArea;

		public Notification(Dimension resolution)
		{
			this.Resolution = resolution;
			Panel = (PanelOverlayElement)OverlayManager.Singleton.CreateOverlayElement("Panel", Guid.NewGuid().ToString());
			TextArea = (TextAreaOverlayElement)OverlayManager.Singleton.CreateOverlayElement("TextArea", Guid.NewGuid().ToString());
			TextArea.MetricsMode = GuiMetricsMode.GMM_PIXELS;
			TextArea.CharHeight = 36;
			TextArea.FontName = "BlueHighway";
			TextArea.Colour = ColourValue.Black;

			Panel.MetricsMode = GuiMetricsMode.GMM_PIXELS;
			Panel.Top = Resolution.Height / 6;
			Panel.Left = 0;
			Panel.Width = Resolution.Width;
			Panel.Height = 50;

			TextArea.Top = 0;
			TextArea.Left = Resolution.Width / 2;
			TextArea.Width = Resolution.Width;
			TextArea.Height = 50;
			TextArea.SetAlignment(TextAreaOverlayElement.Alignment.Center);

			Panel.AddChild(TextArea);
		}

		public override void AddToOverlay(Overlay overlay)
		{
			overlay.Add2D(Panel);
		}

		public override bool CursorCollided(Point cursor)
		{
			return false;
		}

		public void Dispose()
		{
			TextArea.Dispose();
			Panel.Dispose();
		}

		public void DisplayText(string text, float elapsedTime, Action callback)
		{
			if (text == "")
				return;

			// Previous text replaced
			if (this.Callback != null)
				this.Callback();

			RemainingTime = elapsedTime;
			TextArea.Caption = text;
			TextArea.Colour = ColourValue.Black;
			TextArea.SetEnabled(true);
			Panel.SetEnabled(true);
			this.Callback = callback;
		}

		protected override void OnUpdate(UpdateEvent evt)
		{
			if (TextArea.Caption == "")
				return;

			RemainingTime -= evt.ElapsedTime;
			if (RemainingTime <= 0)
			{
				TextArea.Caption = "";
				Callback();
				Callback = null;
				return;
			}
		}
	}
}

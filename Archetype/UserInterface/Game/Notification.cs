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
			TextArea.Colour = ColourValue.White;

			Panel.Top = Resolution.Height / 6;
			Panel.Left = 0;
			Panel.Width = Resolution.Width;
			Panel.Height = 50;

			TextArea.Top = 0;
			TextArea.Left = 0;
			TextArea.Width = Resolution.Width;
			TextArea.Height = 50;
			TextArea.SetAlignment(TextAreaOverlayElement.Alignment.Center);

			Panel.AddChild(TextArea);

			Panel.SetEnabled(false);
			TextArea.SetEnabled(false);
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
			// Previous text replaced
			if (this.Callback != null)
				this.Callback();

			RemainingTime = elapsedTime;
			TextArea.Caption = text;
			TextArea.Colour = ColourValue.White;
			TextArea.SetEnabled(true);
			Panel.SetEnabled(true);
			this.Callback = callback;
		}

		public void Update(UpdateEvent evt)
		{
			if (!TextArea.IsEnabled)
				return;

			RemainingTime -= evt.ElapsedTime;
			if (RemainingTime <= 0)
			{
				TextArea.SetEnabled(false);
				Panel.SetEnabled(false);
				Callback();
				Callback = null;
				return;
			}
		}
	}
}

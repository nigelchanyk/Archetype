using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.UserInterface
{
	public class Button : UserInterfaceComponent, IDisposable
	{
		public PanelOverlayElement Panel { get; private set; }
		public Style PanelStyle { get; private set; }
		public TextAreaOverlayElement TextArea { get; private set; }
		public Style TextAreaStyle { get; private set; }

		public Button(string text, Style panelStyle, Style textAreaStyle)
		{
			Panel = (PanelOverlayElement)OverlayManager.Singleton.CreateOverlayElement("Panel", text + Guid.NewGuid().ToString());
			TextArea = (TextAreaOverlayElement)OverlayManager.Singleton.CreateOverlayElement("TextArea", text + Guid.NewGuid().ToString());

			this.PanelStyle = panelStyle;
			this.TextAreaStyle = textAreaStyle;
			PanelStyle.Apply(Panel);
			TextAreaStyle.Apply(TextArea);

			TextArea.Caption = text;
			TextArea.SetAlignment(TextAreaOverlayElement.Alignment.Left);
		}

		public override void AddToOverlay(Overlay overlay)
		{
			Panel.AddChild(TextArea);
			overlay.Add2D(Panel);
		}

		public override bool CursorCollided(Point cursor)
		{
			return IsWithin(cursor, Panel);
		}

		public void Dispose()
		{
			TextArea.Dispose();
			Panel.Dispose();
		}
	}
}

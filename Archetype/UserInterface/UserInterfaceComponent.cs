using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Archetype.UserInterface
{
	public abstract class UserInterfaceComponent
	{
		protected static bool IsWithin(Point point, OverlayElement element)
		{
			return point.X >= element.Left
				&& point.X <= element.Left + element.Width
				&& point.Y >= element.Top
				&& point.Y <= element.Top + element.Height;
		}

		public event EventHandler Clicked;

		public abstract void AddToOverlay(Overlay overlay);
		public abstract bool CursorCollided(Point cursor);

		public void Click()
		{
			if (Clicked != null)
				Clicked(this, new EventArgs());
		}
	}
}

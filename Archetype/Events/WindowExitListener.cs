using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Archetype.Events
{
	public class WindowExitListener : WindowEventListener
	{
		public event EventHandler WindowClose;

		public override void WindowClosed(RenderWindow rw)
		{
			base.WindowClosed(rw);
			if (WindowClose != null)
				WindowClose(this, new EventArgs());
		}
	}
}

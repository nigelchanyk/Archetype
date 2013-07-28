using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Archetype.Events
{
	public class UpdateEvent
	{
		public MOIS.Keyboard Keyboard { get; private set; }
		public MOIS.Mouse Mouse { get; private set; }
		public float ElapsedTime { get; private set; }

		public UpdateEvent(MOIS.Keyboard keyboard, MOIS.Mouse mouse, FrameEvent frameEvent)
		{
			this.Keyboard = keyboard;
			this.Mouse = mouse;
			this.ElapsedTime = frameEvent.timeSinceLastFrame;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Archetype.UserInterface;

namespace Archetype.Utilities
{
	public static class UserInterfaceHelper
	{
		public static void SetUV(this PanelOverlayElement element, UV uv)
		{
			element.SetUV(uv.U, uv.V, uv.U + uv.Width, uv.V + uv.Height);
		}
	}
}

using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Archetype.Applications;

namespace Archetype
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Application app = new Game();
				app.ShowDisplayConfiguration();
				app.ShowApplication();
			}
			catch (SEHException)
			{
				if (OgreException.IsThrown)
					throw new Exception(OgreException.LastException.FullDescription);
			}
		}
	}
}

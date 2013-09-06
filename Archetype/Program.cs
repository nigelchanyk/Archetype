using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Mogre;

using Archetype.Applications;

namespace Archetype
{
	class Program
	{
		static void Main(string[] args)
		{
			StaticInitializer.Initialize();
			try
			{
				using (Application app = new Game())
				{
					app.ShowDisplayConfiguration();
					app.ShowApplication();
				}
			}
			catch (SEHException)
			{
				if (OgreException.IsThrown)
					throw new Exception(OgreException.LastException.FullDescription);
			}
		}
	}
}

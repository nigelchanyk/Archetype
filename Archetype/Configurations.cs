using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype
{
	class Configurations
	{
		public static Configurations Instance { get; private set; }

		static Configurations()
		{
			Instance = new Configurations();
		}

		public float Sensitivity { get; set; }

		public Configurations()
		{
			Sensitivity = 0.3f;
		}
	}
}

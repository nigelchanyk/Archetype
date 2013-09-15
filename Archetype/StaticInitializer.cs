﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archetype.Utilities;
using Archetype.DataLoaders;

namespace Archetype
{
	public static class StaticInitializer
	{
		public static void Initialize()
		{
			Names.Initialize();
			CharacterConfigurationLoader.Initialize();
			WorldConfigurationLoader.Initialize();
			WeaponLoader.Initialize();
		}
	}
}

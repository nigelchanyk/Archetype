using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Events;
using Archetype.Objects;
using Archetype.Objects.Characters;

namespace Archetype.Controllers
{
	public class Controller
	{
		protected Character Character { get; set; }
		protected World World { get; private set; }

		public Controller(World world)
		{
			this.World = world;
		}

		public virtual void Update(UpdateEvent evt) {}
	}
}

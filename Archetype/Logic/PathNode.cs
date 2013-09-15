using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.Logic
{
	public class PathNode
	{
		public Vector3 Position { get; private set; }
		public PathNode Next { get; set; }

		public PathNode(Vector3 position, PathNode next = null)
		{
			this.Position = position;
			this.Next = next;
		}
	}
}

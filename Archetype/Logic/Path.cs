using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.Logic
{
	public class Path
	{
		public int NodeCount
		{
			get { return PathNodes.Length; }
		}

		public PathNode[] PathNodes { get; private set; }

		public Path(bool cycle, params Vector3[] positions)
			: this(positions, cycle)
		{
		}

		public Path(Vector3[] positions, bool cycle = false)
		{
			PathNodes = new PathNode[positions.Length];

			PathNodes[0] = new PathNode(positions[0]);
			for (int i = 1; i < positions.Length; ++i)
			{
				PathNodes[i] = new PathNode(positions[i]);
				PathNodes[i - 1].Next = PathNodes[i];
			}

			if (cycle)
				PathNodes[positions.Length - 1].Next = PathNodes[0];
		}
	}
}

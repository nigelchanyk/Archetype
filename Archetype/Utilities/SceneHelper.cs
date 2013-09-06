using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.Utilities
{
	public static class SceneHelper
	{
		public static Vector3 ConvertToSpace(this Node source, Node target, Vector3 position)
		{
			return target.ConvertWorldToLocalPosition(source.ConvertLocalToWorldPosition(position));
		}

		public static Vector3 GetWorldPosition(this Node node)
		{
			return node.ConvertLocalToWorldPosition(Vector3.ZERO);
		}

		public static void InvalidateCache(this Node node, bool propagate = false)
		{
			node.Position = node.Position;
			if (propagate)
				node.InvalidateChildrenCache();
		}

		public static void InvalidateChildrenCache(this Node parent)
		{
			foreach (Node node in parent.GetChildIterator())
				node.InvalidateCache(true);
		}

	}
}

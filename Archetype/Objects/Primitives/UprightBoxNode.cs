using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Archetype.Objects.Primitives
{
	public class UprightBoxNode : BoxNode
	{
		public UprightBoxNode(Node referenceNode, Vector3 min, Vector3 max)
			: base(referenceNode, min, max)
		{
		}

		public bool Intersects(UprightCylinderNode cylinder)
		{
			return PrimitiveNode.Intersects(this, cylinder);
		}
	}
}

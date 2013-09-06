using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.Objects.Primitives
{
	public class UprightCylinderNode : PrimitiveNode
	{
		public float Height { get; set; }
		public Vector3 BaseCenterPosition { get; set; }
		public float Radius { get; set; }

		public UprightCylinderNode(Node referenceNode, Vector3 baseCenterPosition, float height, float radius)
			: base(referenceNode)
		{
			this.BaseCenterPosition = baseCenterPosition;
			this.Height = height;
			this.Radius = radius;
		}

		public bool Intersects(UprightBoxNode box)
		{
			return PrimitiveNode.Intersects(box, this);
		}

		public override float? GetIntersectingDistance(Ray ray)
		{
			throw new NotImplementedException();
		}
	}
}

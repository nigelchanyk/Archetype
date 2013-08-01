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
		public float Radius { get; set; }

		public UprightCylinderNode(Node parent, Vector3 position, float height, float radius)
			: base(parent, position, Quaternion.IDENTITY)
		{
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

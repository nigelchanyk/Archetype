using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Archetype.Objects.Primitives
{
	public class BoxNode : PrimitiveNode
	{
		public Vector3 Max { get; private set; }
		public Vector3 Min { get; private set; }

		public BoxNode(Node parent, Vector3 min, Vector3 max)
			: base(parent)
		{
			if (min.x > max.x || min.y > max.y || min.z > max.z)
				throw new ArgumentException("`min` contains at least one coordinate greater than `max`.");
			this.Max = max;
			this.Min = min;
		}

		public override float? GetIntersectingDistance(Ray ray)
		{
			return PrimitiveNode.GetIntersectionDistance(this, ray);
		}

		public bool Intersects(SphereNode sphere)
		{
			return PrimitiveNode.Intersects(this, sphere);
		}

		public AxisAlignedBox ToMogreAxisAlignedBox()
		{
			return new AxisAlignedBox(Min, Max);
		}
	}
}

using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype.Objects.Primitives
{
	public class SphereNode : PrimitiveNode
	{
		public float Radius { get; private set; }

		public SphereNode(Node parent, Vector3 position, Quaternion orientation, float radius)
			: base(parent, position, orientation)
		{
			this.Radius = radius;
		}

		public bool Intersects(BoxNode box)
		{
			return PrimitiveNode.Intersects(box, this);
		}

		public Vector3? Intersects(Ray ray)
		{
			return PrimitiveNode.Intersects(this, ray);
		}

		public bool Intersects(SphereNode sphere)
		{
			return PrimitiveNode.Intersects(this, sphere);
		}

		public Sphere ToMogreSphere()
		{
			return new Sphere(Vector3.ZERO, Radius);
		}

	}
}

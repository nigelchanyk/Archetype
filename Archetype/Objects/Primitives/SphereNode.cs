using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype.Objects.Primitives
{
	public class SphereNode : PrimitiveNode
	{
		public Vector3 Position { get; private set; }
		public float Radius { get; private set; }

		public SphereNode(Node parent, Vector3 position, float radius)
			: base(parent)
		{
			this.Position = position;
			this.Radius = radius;
		}

		public bool Intersects(BoxNode box)
		{
			return PrimitiveNode.Intersects(box, this);
		}

		public override float? GetIntersectingDistance(Ray ray)
		{
			return PrimitiveNode.Intersects(this, ray);
		}

		public bool Intersects(SphereNode sphere)
		{
			return PrimitiveNode.Intersects(this, sphere);
		}

		public Sphere ToMogreSphere()
		{
			return new Sphere(Position, Radius);
		}

	}
}

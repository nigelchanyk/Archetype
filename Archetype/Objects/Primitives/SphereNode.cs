using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.Objects.Primitives
{
	public class SphereNode : PrimitiveNode
	{
		public Vector3 Position { get; private set; }
		public float Radius { get; private set; }

		public SphereNode(Node parent, Vector3 position, float radius)
			: this(parent, null, position, radius)
		{
		}

		public SphereNode(Node parent, Entity referenceWorld, Vector3 position, float radius)
			: base(parent, referenceWorld)
		{
			this.Position = position;
			this.Radius = radius;
		}

		public override float? GetIntersectingDistance(Ray ray)
		{
			return PrimitiveNode.Intersects(this, ray);
		}

		public bool Intersects(BoxNode box)
		{
			return PrimitiveNode.Intersects(box, this);
		}

		public bool Intersects(FrustumNode frustum)
		{
			return PrimitiveNode.Intersects(frustum, this);
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

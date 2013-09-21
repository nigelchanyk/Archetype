using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Archetype.Objects.Primitives
{
	/// <summary>
	/// Frustum node is not part of primitive node because
	/// ray-frustum intersection is not implemented.
	/// </summary>
	public class FrustumNode
	{
		public Camera Camera { get; private set; }
		public Vector3 Position { get { return Camera.RealPosition; } }

		public FrustumNode(Camera camera)
		{
			this.Camera = camera;
		}

		public bool Contains(Vector3 point)
		{
			return Camera.IsVisible(point);
		}

		public bool Intersects(SphereNode sphere)
		{
			return sphere.Intersects(this);
		}
	}
}

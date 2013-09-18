using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Archetype.Objects.Primitives
{
	public class FrustumNode : PrimitiveNode
	{
		public Camera Camera { get; private set; }

		public FrustumNode(Camera camera)
			: base(camera.ParentSceneNode)
		{
			this.Camera = camera;
		}

		public IEnumerable<Plane> GetPlanes()
		{
			for (ushort i = 0; i < 6; ++i)
				yield return Camera.GetFrustumPlane(i);
		}

		public bool Intersects(SphereNode sphere)
		{
			return PrimitiveNode.Intersects(this, sphere);
		}
	}
}

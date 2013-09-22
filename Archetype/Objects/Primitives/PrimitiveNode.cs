using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Utilities;

namespace Archetype.Objects.Primitives
{
	public abstract class PrimitiveNode
	{
		/// <summary>
		/// Get the distance from the starting position of the ray in the box's space.
		/// If there is no scale transformation between the box and the ray, the distance should be the same
		/// as the distance in world space.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		protected static float? GetIntersectionDistance(BoxNode a, Ray b)
		{
			Ray bTransformed = b.TransformRay(a.ReferenceNode);
			Pair<bool, float> result = bTransformed.Intersects(a.ToMogreAxisAlignedBox());
			if (result.first)
				return result.second;
			return null;
		}

		protected static float? Intersects(SphereNode a, Ray b)
		{
			Ray bTransformed = b.TransformRay(a.ReferenceNode);
			Pair<bool, float> result = bTransformed.Intersects(a.ToMogreSphere());
			if (result.first)
				return result.second;
			return null;
		}

		protected static bool Intersects(BoxNode a, SphereNode b)
		{
			Vector3 bTransformedCenter;
			float bTransformedSquaredRadius;
			TransformSphere(b, a.ReferenceNode, out bTransformedCenter, out bTransformedSquaredRadius);
			Vector3 closest = bTransformedCenter.Clamp(a.Min, a.Max);
			return closest.SquaredDistance(bTransformedCenter) < bTransformedSquaredRadius;
		}

		protected static bool Intersects(FrustumNode a, SphereNode b)
		{
			Vector3 bTransformedCenter = b.ReferenceNode.ConvertLocalToWorldPosition(b.Position);
			Vector3 bTransformedTop = b.ReferenceNode.ConvertLocalToWorldPosition(MathHelper.Up * b.Radius);
			float bTransformedRadius = bTransformedCenter.Distance(bTransformedTop);
			return a.Camera.IsVisible(new Sphere(bTransformedCenter, bTransformedRadius));
		}

		protected static bool Intersects(UprightBoxNode a, UprightCylinderNode b)
		{
			Vector3 baseTransformed = a.ReferenceNode.ConvertWorldToLocalPosition(
				b.ReferenceNode.ConvertLocalToWorldPosition(b.BaseCenterPosition)
			);
			// Check if the two objects intersect the same horizontal (xz) plane
			// Simplification:
			// |a.yCenter - b.yCenter| > max(a.height / 2, b.height / 2)
			// |(a.min.y + a.max.y) / 2 - (b.base.y + b.base.y + b.height) / 2| > max((a.max.y - a.min.y) / 2, b.height / 2)
			// |(a.min.y + a.max.y) - (b.base.y * 2 + b.height)| / 2 > max((a.max.y - a.min.y), b.height) / 2
			if (System.Math.Abs(a.Min.y + a.Max.y - (b.BaseCenterPosition.y * 2 + b.Height)) > System.Math.Max(a.Max.y - a.Min.y, b.Height))
				return false;

			// Check if the two objects intersect when projected onto a horizontal (xz) plane
			Vector2 closest = baseTransformed.ToVectorXZ().Clamp(a.Min.ToVectorXZ(), a.Max.ToVectorXZ());
			return closest.SquaredDistance(baseTransformed.ToVectorXZ()) < b.Radius.Squared();
		}

		protected static bool Intersects(SphereNode a, SphereNode b)
		{
			Vector3 bTransformedCenter;
			float bTransformedSquaredRadius;
			TransformSphere(b, a.ReferenceNode, out bTransformedCenter, out bTransformedSquaredRadius);
			// Remember that `a` sits in the origin of its local space, so the squared distance is the center of `b` - (0, 0, 0).
			float squaredDistance = bTransformedCenter.SquaredLength;
			return squaredDistance < System.Math.Min(bTransformedSquaredRadius, a.Radius.Squared());
		}

		private static void TransformSphere(SphereNode sphere, Node destWorld, out Vector3 transformedCenter, out float transformedSquaredRadius)
		{
			transformedCenter = destWorld.ConvertWorldToLocalPosition(sphere.ReferenceNode.ConvertLocalToWorldPosition(Vector3.ZERO));
			Vector3 bTransformedTop = destWorld.ConvertWorldToLocalPosition
			(
				sphere.ReferenceNode.ConvertWorldToLocalPosition(MathHelper.Up * sphere.Radius)
			);
			transformedSquaredRadius = bTransformedTop.SquaredDistance(transformedCenter);
		}


		public Node ReferenceNode { get; private set; }
		// World space of nodes in an entity is the space of entity.
		public Entity ReferenceWorld { get; private set; } // Nullable

		public PrimitiveNode(Node referenceNode, Entity referenceWorld)
		{
			this.ReferenceNode = referenceNode;
			this.ReferenceWorld = referenceWorld;
		}

		public virtual Vector3 GetCenter(bool worldSpace)
		{
			// To be overridden if origin of local space is not the center of the shape.
			if (!worldSpace)
				return ReferenceNode.Position;

			// Double conversion required for nodes within skeletons
			return ReferenceWorld == null ? ReferenceNode.GetWorldPosition() : ReferenceWorld.ParentSceneNode.ConvertLocalToWorldPosition(ReferenceNode.GetWorldPosition());
		}

		public void InvalidateCache()
		{
			ReferenceNode.InvalidateCache();
		}

		public abstract float? GetIntersectingDistance(Ray ray);
	}
}

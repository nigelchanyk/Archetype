using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.Utilities
{
	public static class MathHelper
	{
		public static readonly Random Randomizer = new Random();

		// Directional Constants
		public static readonly Vector3 Backward = Vector3.UNIT_Z;
		public static readonly Vector3 Down = Vector3.NEGATIVE_UNIT_Y;
		public static readonly Vector3 Forward = Vector3.NEGATIVE_UNIT_Z;
		public static readonly Vector3 Left = Vector3.NEGATIVE_UNIT_X;
		public static readonly Vector3 Right = Vector3.UNIT_X;
		public static readonly Vector3 Up = Vector3.UNIT_Y;

		// Angle Constants
		public static readonly float PiOver6 = Mogre.Math.PI / 6;
		public static readonly float PiOver4 = Mogre.Math.PI / 4;
		public static readonly float PiOver3 = Mogre.Math.PI / 3;
		public static readonly float PiOver2 = Mogre.Math.HALF_PI;
		public static readonly float Pi = Mogre.Math.PI;
		public static readonly float TwoPi = Mogre.Math.TWO_PI;

		// Special Constants
		public static readonly float SqrtTwo = (float)System.Math.Sqrt(2);

		public static float Clamp(this float original, float min, float max)
		{
			if (original < min)
				return min;
			if (original > max)
				return max;

			return original;
		}

		public static Vector2 Clamp(this Vector2 original, Vector2 min, Vector2 max)
		{
			return new Vector2(original.x.Clamp(min.x, max.x), original.y.Clamp(min.y, max.y));
		}

		public static Vector3 Clamp(this Vector3 original, Vector3 min, Vector3 max)
		{
			return new Vector3(original.x.Clamp(min.x, max.x), original.y.Clamp(min.y, max.y), original.z.Clamp(min.z, max.z));
		}

		public static Vector3 ConvertLocalToWorldDelta(this Node node, Vector3 delta)
		{
			Vector3 origin = node.ConvertLocalToWorldPosition(Vector3.ZERO);
			Vector3 reference = node.ConvertLocalToWorldPosition(delta);
			return reference - origin;
		}

		/// <summary>
		/// Create an array of vectors that only contains part of the original coordinates.
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static Vector3[] CreatePartialVectorCombinations(this Vector3 vector)
		{
			return new Vector3[]
			{
				vector.Mask(false, false, true),
				vector.Mask(false, true, false),
				vector.Mask(false, true, true),
				vector.Mask(true, false, false),
				vector.Mask(true, false, true),
				vector.Mask(true, true, false)
			};
		}

		public static Quaternion CreateQuaternionFromYawPitchRoll(float yaw, float pitch, float roll)
		{
			Matrix3 rotMatrix = Matrix3.IDENTITY;
			rotMatrix.FromEulerAnglesYXZ(yaw, pitch, roll);
			Quaternion orientation = Quaternion.IDENTITY;
			orientation.FromRotationMatrix(rotMatrix);
			return orientation;
		}

		public static float Lerp(float source, float destination, float amount)
		{
			return source + (destination - source) * amount;
		}

		public static Vector3 Lerp(Vector3 source, Vector3 destination, float amount)
		{
			return source + (destination - source) * amount;
		}

		/// <summary>
		/// Mask out components of the vector.
		/// </summary>
		/// <param name="original">Original vector</param>
		/// <param name="x">If false, x will be set to 0.</param>
		/// <param name="y">If false, y will be set to 0.</param>
		/// <param name="z">If false, z will be set to 0.</param>
		/// <returns>Masked vector</returns>
		public static Vector3 Mask(this Vector3 original, bool x, bool y, bool z)
		{
			return new Vector3(x ? original.x : 0, y ? original.y : 0, z ? original.z : 0);
		}

		public static float NextFloat(this Random randomizer)
		{
			return (float)randomizer.NextDouble();
		}

		public static float NextFloat(this Random randomizer, float lowerBound, float upperBound)
		{
			return randomizer.NextFloat() * (upperBound - lowerBound) + lowerBound;
		}

		public static float Squared(this float value)
		{
			return value * value;
		}

		public static int Squared(this int value)
		{
			return value * value;
		}

		public static float SquaredDistance(this Vector2 a, Vector2 b)
		{
			return (b - a).SquaredLength;
		}

		public static float SquaredDistance(this Vector3 a, Vector3 b)
		{
			return (b - a).SquaredLength;
		}

		public static SphericalCoordinate ToSphericalCoordinate(this Vector3 vector)
		{
			float radius = vector.Length;
			if (radius == 0)
				return new SphericalCoordinate(0, 0, 0);

			float s = new Vector2(vector.x, vector.y).Length;
			float phi = (float)System.Math.Acos(vector.z / radius);
			float theta = s == 0 ? 0 : (float)System.Math.Asin(vector.y / s);
			if (vector.x < 0)
				theta = MathHelper.Pi - theta;

			return new SphericalCoordinate(radius, phi, theta);
		}

		public static Vector2 ToVectorXZ(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.z);
		}

		/// <summary>
		/// Transform a given delta in local space to delta in parent space.
		/// </summary>
		/// <param name="delta">Delta in local space</param>
		/// <param name="position">Position of local space relative to parent space.</param>
		/// <param name="scale">Scale of local space relative to parent space.</param>
		/// <param name="orientation">Orientation of local space relative to parent space.</param>
		/// <returns>Delta in parent space</returns>
		public static Vector3 TransformDelta(this Vector3 delta, Vector3 position, Vector3 scale, Quaternion orientation)
		{
			Matrix4 transform = Matrix4.IDENTITY;
			transform.MakeTransform(position, scale, orientation);
			return transform.TransformAffine(delta) - transform.TransformAffine(Vector3.ZERO);
		}

		public static Ray TransformRay(this Ray ray, Node world)
		{
			Vector3 transformedOrigin = world.ConvertWorldToLocalPosition(ray.Origin);
			Vector3 transformedDirection = world.ConvertWorldToLocalPosition(ray.Direction + ray.Origin) - transformedOrigin;
			return new Ray(transformedOrigin, transformedDirection);
		}
	}
}

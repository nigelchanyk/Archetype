using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Math = System.Math;

namespace Archetype.Utilities
{
	public struct SphericalCoordinate
	{
		public float Radius { get { return _radius; } }
		public float Phi { get { return _phi; } }
		public float Theta { get { return _theta; } }

		private float _radius;
		private float _phi;
		private float _theta;

		public SphericalCoordinate(float radius, float phi, float theta)
		{
			_radius = radius;
			_phi = phi;
			_theta = theta;
		}

		public Vector3 ToVector3()
		{
			float sinTheta = (float)Math.Sin(Theta);
			float cosTheta = (float)Math.Cos(Theta);
			float sinPhi = (float)Math.Sin(Phi);
			float cosPhi = (float)Math.Cos(Phi);

			return new Vector3(Radius * sinPhi * cosTheta, Radius * sinPhi * sinTheta, Radius * cosPhi);
		}

	}
}

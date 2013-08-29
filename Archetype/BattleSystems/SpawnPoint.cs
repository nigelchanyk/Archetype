using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Archetype.BattleSystems
{
	public struct SpawnPoint
	{
		public float X { get { return _x; } }
		public float Z { get { return _z; } }

		private float _x;
		private float _z;

		public SpawnPoint(float x, float z)
		{
			_x = x;
			_z = z;
		}

		public Vector3 ToVector3()
		{
			return new Vector3(X, 0, Z);
		}
	}
}

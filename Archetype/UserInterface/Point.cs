using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archetype.Utilities;

namespace Archetype.UserInterface
{
	public struct Point
	{
		public static bool operator ==(Point a, Point b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(Point a, Point b)
		{
			return !(a == b);
		}

		public int X { get { return _x; } }
		public int Y { get { return _y; } }

		private int _x;
		private int _y;

		public Point(int x, int y)
		{
			_x = x;
			_y = y;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj is Point)
				return this == (Point)obj;
			return false;
		}

		public override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 23 + X.GetHashCode();
			hash = hash * 23 + Y.GetHashCode();

			return hash;
		}
	}
}

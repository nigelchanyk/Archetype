﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype.UserInterface
{
	public class UV
	{
		public static UV[] CreateFromHorizontalGrid(int division, float minV = 0, float height = 1)
		{
			if (division <= 0)
				throw new ArgumentException("Number of division must be greater than zero.");

			UV[] results = new UV[division];
			float width = 1f / division;
			for (int i = 0; i < division; ++i)
				results[i] = new UV(i * width, minV, width, height);
			return results;
		}

		public static UV[][] CreateFromGrid(int verticalDivision, int horizontalDivision)
		{
			if (verticalDivision <= 0)
				throw new ArgumentException("Number of division must be greater than zero.");

			UV[][] results = new UV[verticalDivision][];
			float height = 1f / verticalDivision;
			for (int i = 0; i < verticalDivision; ++i)
				results[i] = CreateFromHorizontalGrid(horizontalDivision, i * height, height);
			return results;
		}

		public float Height { get { return _height; } }
		public float U { get { return _u; } }
		public float V { get { return _v; } }
		public float Width { get { return _width; } }

		private float _height;
		private float _u;
		private float _v;
		private float _width;

		public UV(float u, float v, float width, float height)
		{
			_u = u;
			_v = v;
			_width = width;
			_height = height;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype.UserInterface
{
	public struct Dimension
	{
		public int Width { get { return _width; } }
		public int Height { get { return _height; } }

		private int _width;
		private int _height;

		public Dimension(int width, int height)
		{
			_width = width;
			_height = height;
		}
	}
}

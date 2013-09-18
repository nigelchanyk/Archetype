using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Archetype.Cameras
{
	public class CameraManager
	{
		public event EventHandler CameraChanged;

		public Camera ActiveCamera
		{
			get { return _camera; }
			set
			{
				_camera = value;
				CameraChanged(this, new EventArgs());
			}
		}

		private Camera _camera;
	}
}

using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Events;
using Archetype.Objects;
using Archetype.UserInterface;

namespace Archetype.Controllers
{
	public class CameraController : Controller
	{
		public bool CameraEnabled { get; set; }

		protected Camera Camera
		{
			get
			{
				if (CameraEnabled)
					return World.Camera;

				throw new FieldAccessException("Camera is inaccessible when disabled.");
			}
		}
		protected Point WindowCenter { get; private set; }

		public CameraController(World world, Point windowCenter, bool cameraEnabled)
			: base(world)
		{
			this.CameraEnabled = cameraEnabled;
			this.WindowCenter = windowCenter;
		}

		public override void Update(UpdateEvent evt)
		{
			base.Update(evt);
		}
	}
}

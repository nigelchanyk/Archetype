using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Events;
using Archetype.Objects;

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

		public CameraController(World world, bool cameraEnabled)
			: base(world)
		{
			this.CameraEnabled = cameraEnabled;
		}

		public override void Update(UpdateEvent evt)
		{
			base.Update(evt);
		}
	}
}

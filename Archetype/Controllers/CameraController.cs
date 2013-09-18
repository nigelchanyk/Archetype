using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Cameras;
using Archetype.Events;
using Archetype.Objects;
using Archetype.Objects.Characters;
using Archetype.UserInterface;

namespace Archetype.Controllers
{
	public class CameraController : Controller
	{

		public Camera Camera
		{
			get
			{
				if (Character != null)
					return Character.Camera;

				throw new FieldAccessException("Character is not available. As a result, camera is inaccessible.");
			}
		}
		public bool CameraEnabled
		{
			get
			{
				return _cameraEnabled;
			}
			set
			{
				_cameraEnabled = value;

				if (Character != null)
				{
					Character.FirstPerson = _cameraEnabled;
					if (_cameraEnabled)
						CameraManager.ActiveCamera = Character.Camera;
				}

			}
		}
		public CameraManager CameraManager { get; private set; }

		public new Character Character
		{
			get { return base.Character; }
			set
			{
				// Set previous character to third person
				if (Character != null)
					Character.FirstPerson = false;

				base.Character = value;

				Character.FirstPerson = CameraEnabled;
			}
		}

		protected Point WindowCenter { get; private set; }

		private bool _cameraEnabled;

		public CameraController(World world, CameraManager cameraManager, Point windowCenter, bool cameraEnabled)
			: base(world)
		{
			this.CameraManager = cameraManager;
			this.CameraEnabled = cameraEnabled;
			this.WindowCenter = windowCenter;
		}

		public override void Update(UpdateEvent evt)
		{
			base.Update(evt);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

using Archetype.Events;
using Archetype.Objects;
using Archetype.Utilities;

namespace Archetype.Controllers
{
	public class Player : CameraController
	{

		public Player(World world, bool cameraEnabled)
			: base(world, cameraEnabled)
		{
			Character = world.CreateCharacter();
			Character.Visible = false;
			Character.Position = new Vector3(3, 0, 5);
			Character.LookAt(0, 0, 0);
			Character.AttachCamera(world.Camera);
		}

		public override void Update(UpdateEvent evt)
		{
			base.Update(evt);
			if (!CameraEnabled)
				return;

			Rotate(evt);
			Walk(evt);
			Jump(evt);
		}

		private void Jump(UpdateEvent evt)
		{
			if (evt.Keyboard.IsKeyDown(MOIS.KeyCode.KC_SPACE))
				Character.Jump();
		}

		private void Rotate(UpdateEvent evt)
		{
			Character.Yaw -= evt.Mouse.MouseState.X.rel * Configurations.Instance.Sensitivity * evt.ElapsedTime;
			Character.EyePitch -= evt.Mouse.MouseState.Y.rel * Configurations.Instance.Sensitivity * evt.ElapsedTime;
		}

		private void Walk(UpdateEvent evt)
		{
			Vector3 direction = Vector3.ZERO;
			if (evt.Keyboard.IsKeyDown(MOIS.KeyCode.KC_W))
				direction += MathHelper.Forward;
			if (evt.Keyboard.IsKeyDown(MOIS.KeyCode.KC_S))
				direction += MathHelper.Backward;
			if (evt.Keyboard.IsKeyDown(MOIS.KeyCode.KC_A))
				direction += MathHelper.Left;
			if (evt.Keyboard.IsKeyDown(MOIS.KeyCode.KC_D))
				direction += MathHelper.Right;

			Character.Walk(direction);
		}
	}
}

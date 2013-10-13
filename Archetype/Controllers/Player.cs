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
using Archetype.Utilities;

namespace Archetype.Controllers
{
	public class Player : CameraController
	{
		public Player(World world, CameraManager cameraManager, Point windowCenter, bool cameraEnabled)
			: base(world, cameraManager, windowCenter, cameraEnabled)
		{
		}

		public override void Update(UpdateEvent evt)
		{
			base.Update(evt);
			if (!CameraEnabled || Character == null)
				return;

			Rotate(evt);
			Walk(evt);
			Jump(evt);
			RegularAttack(evt);
			SpecialMove(evt);
		}

		private void RegularAttack(UpdateEvent evt)
		{
			if (evt.Mouse.MouseState.ButtonDown(MOIS.MouseButtonID.MB_Left))
				Character.RegularAttack();
		}

		private void Jump(UpdateEvent evt)
		{
			if (evt.Keyboard.IsKeyDown(MOIS.KeyCode.KC_SPACE))
				Character.Jump();
		}

		private void Rotate(UpdateEvent evt)
		{
			Character.Yaw -= (evt.Mouse.MouseState.X.abs - WindowCenter.X) * Configurations.Instance.Sensitivity * evt.ElapsedTime;
			Character.EyePitch -= (evt.Mouse.MouseState.Y.abs - WindowCenter.Y) * Configurations.Instance.Sensitivity * evt.ElapsedTime;
		}

		private void SpecialMove(UpdateEvent evt)
		{
			if (evt.Keyboard.IsKeyDown(MOIS.KeyCode.KC_Z))
				Character.SpecialMove(0);
			if (evt.Keyboard.IsKeyDown(MOIS.KeyCode.KC_X))
				Character.SpecialMove(1);
			if (evt.Keyboard.IsKeyDown(MOIS.KeyCode.KC_C))
				Character.SpecialMove(2);
		}

		private void Walk(UpdateEvent evt)
		{
			int forwardScalar = 0;
			int rightScalar = 0;

			if (evt.Keyboard.IsKeyDown(MOIS.KeyCode.KC_W))
				forwardScalar++;
			if (evt.Keyboard.IsKeyDown(MOIS.KeyCode.KC_S))
				forwardScalar--;
			if (evt.Keyboard.IsKeyDown(MOIS.KeyCode.KC_A))
				rightScalar--;
			if (evt.Keyboard.IsKeyDown(MOIS.KeyCode.KC_D))
				rightScalar++;

			if (forwardScalar != 0 || rightScalar != 0)
				Character.Walk(MathHelper.Forward * forwardScalar + MathHelper.Right * rightScalar);
			else
				Character.Stop();
		}
	}
}

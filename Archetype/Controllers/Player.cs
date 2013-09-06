using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Events;
using Archetype.Objects;
using Archetype.Objects.Characters;
using Archetype.UserInterface;
using Archetype.Utilities;

namespace Archetype.Controllers
{
	public class Player : CameraController
	{
		public new Character Character
		{
			get { return base.Character; }
			set
			{
				// Set previous character to third person
				if (Character != null)
					Character.FirstPerson = false;

				base.Character = value;
				Character.AttachCamera(World.Camera);
				Character.FirstPerson = true;
			}
		}

		public Player(World world, Point windowCenter, bool cameraEnabled)
			: base(world, windowCenter, cameraEnabled)
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

		private void Walk(UpdateEvent evt)
		{
			Mogre.Vector3 direction = Mogre.Vector3.ZERO;
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

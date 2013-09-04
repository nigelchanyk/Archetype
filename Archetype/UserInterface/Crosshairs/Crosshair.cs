using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Objects.Characters;
using Archetype.Events;

namespace Archetype.UserInterface.Crosshairs
{
	public class Crosshair : UserInterfaceComponent, IDisposable
	{
		public Camera Camera { get; set; }
		public Dimension Resolution { get; set; }
		public Character Character
		{
			get
			{
				return _character;
			}
			set
			{
				_character = value;
				ResetCrosshair();
			}
		}
		public float Radius { get; set; }

		private CrosshairStyler Styler; // Nullable
		private Character _character; // Nullable
		private PanelOverlayElement Container;

		public Crosshair(Dimension resolution, Camera camera)
		{
			this.Resolution = resolution;
			this.Camera = camera;
			Container = (PanelOverlayElement)OverlayManager.Singleton.CreateOverlayElement(
				"Panel",
				"CrosshairContainer" + Guid.NewGuid().ToString()
			);
			Container.MetricsMode = GuiMetricsMode.GMM_PIXELS;
			Container.Top = 0;
			Container.Left = 0;
			Container.Width = 0;
			Container.Height = 0;
		}

		public void Dispose()
		{
			DisposeStyler();
			Container.Dispose();
		}

		protected override void OnUpdate(UpdateEvent evt)
		{
			base.OnUpdate(evt);
			if (Styler != null)
			{
				if (Character.ActiveWeaponHandler == null)
					Styler.Visible = false;
				else
				{
					Styler.Visible = true;
					Styler.Update(Resolution, Character.ActiveWeaponHandler.Inaccuracy, Camera.FOVy.ValueRadians);
				}
			}
			else if (Character.ActiveWeaponHandler != null)
				ResetCrosshair();
		}

		private void DisposeStyler()
		{
			if (Styler != null)
			{
				Styler.Dispose();
				Styler = null;
			}
		}

		private void ResetCrosshair()
		{
			DisposeStyler();
			if (Character != null && Character.Weapon != null)
				Styler = CrosshairStyler.Create(Character.Weapon.WeaponKind, Container);
		}

		public override void AddToOverlay(Overlay overlay)
		{
			overlay.Add2D(Container);
		}

		public override bool CursorCollided(Point cursor)
		{
			return false;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Objects.Weapons;

namespace Archetype.UserInterface.Crosshairs
{
	public abstract class CrosshairStyler : IDisposable
	{
		public static CrosshairStyler Create(Weapon.Kind kind, PanelOverlayElement parentElement)
		{
			switch (kind)
			{
				case Weapon.Kind.Pistol:
					return new RegularCrosshairStyler("RegularCrosshair", parentElement);
			}

			throw new NotSupportedException(kind.ToString() + " not implemented.");
		}

		public abstract bool Visible { get; set; }

		public void Dispose()
		{
			OnDispose();
		}

		public void Update(Dimension resolution, float radius, float fieldOfViewY)
		{
			OnUpdate(resolution, radius, fieldOfViewY);
		}

		protected virtual void OnDispose() {}
		protected virtual void OnUpdate(Dimension resolution, float radius, float fieldOfViewY) {}
	}
}

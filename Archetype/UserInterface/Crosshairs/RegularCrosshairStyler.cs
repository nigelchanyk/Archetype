using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Utilities;

namespace Archetype.UserInterface.Crosshairs
{
	public class RegularCrosshairStyler : CrosshairStyler
	{
		private static readonly float[,] LeftMultipliers = new float[,]
		{
			{ -MathHelper.SqrtTwo, 0, MathHelper.SqrtTwo },
			{ -1, 0, 1 },
			{ -MathHelper.SqrtTwo, 0, MathHelper.SqrtTwo }
		};
		private static readonly int[] LeftOffset = new int[] { -32, -16, 0 };
		private static readonly UV[][] StandardCrosshairUV = UV.CreateFromGrid(3, 3);
		private static readonly float[,] TopMultipliers = new float[,]
		{
			{ -MathHelper.SqrtTwo, -1, -MathHelper.SqrtTwo },
			{ 0, 0, 0 },
			{ MathHelper.SqrtTwo, 1, MathHelper.SqrtTwo }
		};
		public static readonly int[] TopOffset = new int[] { -32, -16, 0 };
		public override bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				if (_visible == value)
					return;

				_visible = value;
				foreach (PanelOverlayElement element in Elements)
				{
					if (value)
						element.Show();
					else
						element.Hide();
				}
			}
		}

		private PanelOverlayElement[,] Elements = new PanelOverlayElement[3, 3];
		private bool _visible = true;

		public RegularCrosshairStyler(string materialName, PanelOverlayElement parentElement)
		{
			for (int x = 0; x < 3; ++x)
			{
				for (int y = 0; y < 3; ++y)
				{
					var element = (PanelOverlayElement)OverlayManager.Singleton.CreateOverlayElement(
						"Panel",
						"RegularCrosshair" + Guid.NewGuid().ToString()
					);
					element.SetUV(StandardCrosshairUV[x][y]);
					element.MetricsMode = GuiMetricsMode.GMM_PIXELS;
					element.MaterialName = materialName;
					element.Width = 32;
					element.Height = 32;
					Elements[x, y] = element;
					parentElement.AddChild(element);
				}
			}
		}

		protected override void OnDispose()
		{
			foreach (PanelOverlayElement element in Elements)
				element.Dispose();
		}

		protected override void OnUpdate(Dimension resolution, float radius, float fieldOfViewY)
		{
			base.OnUpdate(resolution, radius, fieldOfViewY);
			float screenRadius = resolution.Height * radius / fieldOfViewY;
			for (int x = 0; x < 3; ++x)
			{
				for (int y = 0; y < 3; ++y)
				{
					Elements[x, y].Left = resolution.Width / 2 + screenRadius * LeftMultipliers[x, y] + LeftOffset[y];
					Elements[x, y].Top = resolution.Height / 2 + screenRadius * TopMultipliers[x, y] + TopOffset[x];
				}
			}
		}
	}
}

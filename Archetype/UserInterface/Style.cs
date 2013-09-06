using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.UserInterface
{
	public class Style
	{
		public ColourValue? Color { get; set; }
		public Dimension? Dimension { get; set; }
		public string Font { get; set; }
		public int? FontSize { get; set; }
		public Point? Position { get; set; }
		public string Material { get; set; }

		public void Apply(TextAreaOverlayElement element)
		{
			// Apply base first, otherwise the metrics mode won't be correct.
			ApplyBase(element);
			element.FontName = Font ?? "BlueHighway";
			element.CharHeight = FontSize ?? 20;
		}

		public void Apply(PanelOverlayElement element)
		{
			ApplyBase(element);
		}

		private void ApplyBase(OverlayElement element)
		{
			element.MetricsMode = GuiMetricsMode.GMM_PIXELS;
			element.Colour = Color ?? ColourValue.Black;
			element.Width = Dimension != null ? Dimension.Value.Width : 50;
			element.Height = Dimension != null ? Dimension.Value.Height : 50;
			element.Left = Position != null ? Position.Value.X : 0;
			element.Top = Position != null ? Position.Value.Y : 0;
			element.MaterialName = Material ?? "";
		}
	}
}

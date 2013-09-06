using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MOIS;
using Mogre;

using Archetype.Utilities;

namespace Archetype.UserInterface
{
	public class CursorOverlay : IDisposable
	{
		public enum Kind
		{
			Default
		}

		private static readonly Dictionary<Kind, UV> MaterialMapper = new Dictionary<Kind, UV>();

		static CursorOverlay()
		{
			UV[] uv = UV.CreateFromHorizontalGrid(8);
			MaterialMapper.Add(Kind.Default, uv[0]);
		}

		public Kind CursorKind
		{
			get
			{
				return _kind;
			}
			set
			{
				_kind = value;
				Cursor.SetUV(MaterialMapper[value]);
			}
		}
		public bool Visible
		{
			get
			{
				return Overlay.IsVisible;
			}
			set
			{
				if (value)
					Overlay.Show();
				else
					Overlay.Hide();
			}
		}
		public ushort ZOrder
		{
			get { return Overlay.ZOrder; }
			set { Overlay.ZOrder = value; }
		}

		private PanelOverlayElement Cursor;
		private Overlay Overlay;
		private Kind _kind = Kind.Default;

		public CursorOverlay(ushort zOrder)
		{
			Overlay = OverlayManager.Singleton.Create("CursorOverlay" + Guid.NewGuid().ToString());
			Cursor = (PanelOverlayElement)OverlayManager.Singleton.CreateOverlayElement("Panel", "Cursor" + Guid.NewGuid().ToString());
			Cursor.MetricsMode = GuiMetricsMode.GMM_PIXELS;
			Cursor.Width = 32;
			Cursor.Height = 32;
			Cursor.MaterialName = "Cursor";
			// Set Material
			CursorKind = Kind.Default;
			Overlay.Add2D(Cursor);
			Overlay.ZOrder = zOrder;
			Overlay.Show();
		}

		public void Dispose()
		{
			Cursor.Dispose();
			Overlay.Dispose();
		}

		public void MouseMove(MouseEvent evt)
		{
			Cursor.Left = evt.state.X.abs;
			Cursor.Top = evt.state.Y.abs;
		}
	}
}

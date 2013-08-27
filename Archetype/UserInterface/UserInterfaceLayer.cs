using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MOIS;

namespace Archetype.UserInterface
{
	public class UserInterfaceLayer : IDisposable
	{
		public ushort ZOrder
		{
			get { return Overlay.ZOrder; }
			set { Overlay.ZOrder = value; }
		}

		private List<UserInterfaceComponent> Components = new List<UserInterfaceComponent>();
		private Overlay Overlay;
		private Point PreviousCursorPosition = new Point(int.MinValue, int.MinValue);

		public UserInterfaceLayer(ushort zOrder)
		{
			Overlay = OverlayManager.Singleton.Create("Overlay" + Guid.NewGuid());
			Overlay.ZOrder = zOrder;
			Overlay.Show();
		}

		public void Add(UserInterfaceComponent component)
		{
			component.AddToOverlay(Overlay);
			Components.Add(component);
		}

		public void Dispose()
		{
			Overlay.Dispose();
		}

		public void MousePress(MouseEvent evt, MouseButtonID id)
		{
			if (id != MouseButtonID.MB_Left)
				return;
			PreviousCursorPosition = ConvertToPoint(evt);
		}

		public void MouseRelease(MouseEvent evt, MouseButtonID id)
		{
			if (id != MouseButtonID.MB_Left)
				return;
			if (PreviousCursorPosition == ConvertToPoint(evt))
				Components.ForEach(x => x.Click());

			PreviousCursorPosition = new Point(int.MinValue, int.MinValue);
		}

		private Point ConvertToPoint(MouseEvent evt)
		{
			return new Point((int)evt.state.X.abs, (int)evt.state.Y.abs);
		}
	}
}

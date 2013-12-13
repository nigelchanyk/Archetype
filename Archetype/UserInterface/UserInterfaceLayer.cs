﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MOIS;
using Mogre;

using Archetype.Events;

namespace Archetype.UserInterface
{
	public class UserInterfaceLayer : IDisposable
	{
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
			Components.AddRange(component.GetAllComponents());
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
			Point releasePosition = ConvertToPoint(evt);
			UserInterfaceComponent clickedComponent = Components.FirstOrDefault(
				x => x.CursorCollided(PreviousCursorPosition) && x.CursorCollided(releasePosition)
			);
			if (clickedComponent != null)
				clickedComponent.Click();

			PreviousCursorPosition = new Point(int.MinValue, int.MinValue);
		}

		public void Update(UpdateEvent evt)
		{
			foreach (UserInterfaceComponent component in Components)
				component.Update(evt);
		}

		private Point ConvertToPoint(MouseEvent evt)
		{
			return new Point((int)evt.state.X.abs, (int)evt.state.Y.abs);
		}
	}
}

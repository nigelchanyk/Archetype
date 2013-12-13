using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.UserInterface
{
	public class Menu : UserInterfaceComponent, IDisposable
	{
		public Point Position { get; private set; }

		private List<Button> Buttons;

		public Menu(Point position, int capacity)
		{
			this.Position = position;
			this.Buttons = new List<Button>(capacity);
		}

		public void Add(string item, EventHandler handler)
		{
			if (Buttons.Count == Buttons.Capacity)
				throw new ArgumentException("Maximum menu capacity exceeded.");

			Button button = new Button(
				item,
				new Style
				{
					Dimension = new Dimension(200, 50),
					Position = new Point(Position.X, Position.Y + Buttons.Count * 65),
					Material = "Core/StatsBlockCenter"
				},
				new Style
				{
					Dimension = new Dimension(160, 40),
					Position = new Point(10, 20),
					FontSize = 26,
					Color = ColourValue.White,
					Font = "BlueHighway"
				}
			);
			button.Clicked += handler;
			Buttons.Add(button);
		}

		public override void AddToOverlay(Overlay overlay)
		{
			Buttons.ForEach(b => b.AddToOverlay(overlay));
		}

		public override bool CursorCollided(Point cursor)
		{
			return false;
		}

		public void Dispose()
		{
			Buttons.ForEach(b => b.Dispose());
		}

		public override IEnumerable<UserInterfaceComponent> GetAllComponents()
		{
			return Buttons;
		}
	}
}

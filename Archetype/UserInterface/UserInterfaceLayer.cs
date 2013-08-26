using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miyagi.UI;
using Archetype.AssetManagers;
using Miyagi.Common.Data;

namespace Archetype.UserInterface
{
	public class UserInterfaceLayer : GUI
	{
		public CursorCollection CursorCollection { get; private set; }
		public FontCollection FontCollection { get; private set; }
		public SkinCollection SkinCollection { get; private set; }

		public UserInterfaceLayer(FontCollection fontCollection, SkinCollection skinCollection, CursorCollection cursorCollection)
		{
			this.FontCollection = fontCollection;
			this.SkinCollection = skinCollection;
			this.CursorCollection = cursorCollection;
			
		}

		public void SetCursor(string cursorName)
		{
			if (MiyagiSystem.GUIManager.Cursor != null)
				MiyagiSystem.GUIManager.Cursor.Visible = false;
			MiyagiSystem.GUIManager.Cursor = CursorCollection[cursorName];
			MiyagiSystem.GUIManager.Cursor.Visible = true;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miyagi.UI;
using Miyagi.Common;
using Miyagi.Common.Resources;
using Miyagi.Common.Data;

namespace Archetype.AssetManagers
{
	public class CursorCollection : IDisposable
	{
		public Dictionary<string, Cursor> CursorMapper = new Dictionary<string,Cursor>();

		public CursorCollection(string xmlFilePath, MiyagiSystem system)
		{
			foreach (Skin skin in Skin.CreateFromXml(xmlFilePath, system))
				CursorMapper.Add(skin.Name, new Cursor(skin, new Size(50, 50), new Point(0, 0), false));
		}

		public Cursor this[string name]
		{
			get { return CursorMapper[name]; }
		}

		public void Dispose()
		{
			foreach (Cursor cursor in CursorMapper.Values)
				cursor.Dispose();
		}
	}
}

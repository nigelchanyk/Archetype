using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miyagi.Common.Resources;
using Miyagi.Common;

namespace Archetype.AssetManagers
{
	public class FontCollection
	{
		private Dictionary<string, Font> FontMapper = new Dictionary<string, Font>();

		public FontCollection(string xmlFilePath, MiyagiSystem system)
		{
			foreach (Font font in ImageFont.CreateFromXml(xmlFilePath, system))
				FontMapper.Add(font.Name, font);
		}

		public Font this[string name]
		{
			get { return FontMapper[name]; }
		}
	}
}

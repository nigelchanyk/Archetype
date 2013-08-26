using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miyagi.Common;
using Miyagi.Common.Resources;

namespace Archetype.AssetManagers
{
	public class SkinCollection
	{
		private Dictionary<string, Skin> SkinMapper = new Dictionary<string, Skin>();
		
		public SkinCollection(string xmlFilePath, MiyagiSystem system)
		{
			foreach (Skin skin in Skin.CreateFromXml(xmlFilePath, system))
				SkinMapper.Add(skin.Name, skin);
		}

		public Skin this[string name]
		{
			get { return SkinMapper[name]; }
		}
	}
}

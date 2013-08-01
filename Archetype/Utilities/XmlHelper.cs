using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using System.Xml.Linq;

namespace Archetype.Utilities
{
	public static class XmlHelper
	{
		public static Vector3 ParseXYZ(this XElement element, Vector3 defaultValue)
		{
			return new Vector3
			(
				element.Attribute("x").ToFloat(defaultValue.x),
				element.Attribute("y").ToFloat(defaultValue.y),
				element.Attribute("z").ToFloat(defaultValue.z)
			);
		}

		public static float ToFloat(this XAttribute attribute, float defaultValue)
		{
			return (float?)attribute ?? defaultValue;
		}
	}
}

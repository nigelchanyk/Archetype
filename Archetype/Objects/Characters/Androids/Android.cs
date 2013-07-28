using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Assets;

namespace Archetype.Objects.Characters.Androids
{
	public abstract class Android : Character
	{
		private static readonly String[] MaleBodyEntityNames =
		{
			AssetCollections.AlphaJoints,
			AssetCollections.AlphaLimbs,
			AssetCollections.AlphaTorso
		};

		public Android(World world)
			: base(world, MaleBodyEntityNames)
		{

		}
	}
}

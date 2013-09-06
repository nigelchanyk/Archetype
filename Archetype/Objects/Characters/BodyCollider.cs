using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Objects.Primitives;

namespace Archetype.Objects.Characters
{
	public class BodyCollider
	{
		public float DamageMultiplier { get; private set; }
		public PrimitiveNode PrimitiveNode { get; private set; }

		public BodyCollider(PrimitiveNode primitiveNode, float damageMultiplier)
		{
			this.PrimitiveNode = primitiveNode;
			this.DamageMultiplier = damageMultiplier;
		}
	}
}

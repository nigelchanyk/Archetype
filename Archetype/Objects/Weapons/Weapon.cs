using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype.Objects.Weapons
{
	public class Weapon
	{
		public float AttackInterval { get; private set; }
		public int BaseDamage { get; private set; }
		public string ModelName { get; private set; }

		public Weapon(string modelName, int baseDamage, float attackInterval)
		{
			this.ModelName = modelName;
			this.BaseDamage = baseDamage;
			this.AttackInterval = attackInterval;
		}
	}
}

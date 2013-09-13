using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.Objects.Weapons
{
	public class Weapon
	{
		public enum Kind
		{
			Pistol,
			SubmachineGun,
			Rifle,
			SniperRifle,
			MachineGun,
			Sword,
			Scythe,
			Dagger
		}

		public float AttackInterval { get; private set; }
		public string AttackSound { get; private set; }
		public int BaseDamage { get; private set; }
		public Vector3 FirstPersonPosition { get; private set; }
		public string ModelName { get; private set; }
		public string Name { get; private set; }
		public Kind WeaponKind { get; private set; }

		public Weapon(Kind kind, string name, string modelName, int baseDamage, float attackInterval, string attackSound, Vector3 firstPersonPosition)
		{
			this.WeaponKind = kind;
			this.Name = name;
			this.ModelName = modelName;
			this.BaseDamage = baseDamage;
			this.AttackInterval = attackInterval;
			this.AttackSound = attackSound;
			this.FirstPersonPosition = firstPersonPosition;
		}
	}
}

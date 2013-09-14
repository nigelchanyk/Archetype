using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Mogre;

using Archetype.Objects.Weapons;
using Archetype.Utilities;

using MogreMath = Mogre.Math;

namespace Archetype.DataLoaders
{
	public class WeaponLoader
	{
		private static readonly Dictionary<string, RangedWeapon> RangedWeaponMapper = new Dictionary<string, RangedWeapon>();

		public static RangedWeapon Get(string name)
		{
			return RangedWeaponMapper[name];
		}

		public static IEnumerable<string> GetWeaponNames()
		{
			return RangedWeaponMapper.Keys;
		}

		public static void Initialize()
		{
			XElement root = XElement.Load("Assets/Data/Weapons.xml");
			LoadRangedWeapons(root);
		}

		private static void LoadRangedWeapons(XElement root)
		{
			foreach (XElement weaponElement in root.Element("RangedWeapons").Elements("Weapon"))
			{
				WeaponAttributes attributes = LoadRangedWeaponAttributes(weaponElement);
				RangedWeaponMapper.Add(
					attributes.Name,
					new RangedWeapon(
						attributes.Kind,
						attributes.Name,
						attributes.ModelName,
						attributes.BaseDamage,
						attributes.AttackInterval,
						attributes.AttackSound,
						attributes.FirstPersonPosition,
						attributes.MinInaccuracy,
						attributes.MaxInaccuracy,
						attributes.InaccuracyGrowth,
						attributes.MaxRecoil,
						attributes.RecoilGrowth,
						attributes.FirstPersonMuzzleFlashPosition
					)
				);
			}
		}

		private static WeaponAttributes LoadRangedWeaponAttributes(XElement weaponElement)
		{
			WeaponAttributes attributes = LoadWeaponAttributes(weaponElement);
			XElement inaccuracyElement = weaponElement.Element("Inaccuracy");
			XElement recoilElement = weaponElement.Element("Recoil");
			XElement firstPersonElement = weaponElement.Element("FirstPerson");
			XElement firstPersonMuzzleFlashElement = firstPersonElement.Element("MuzzleFlash");

			attributes.MinInaccuracy = MogreMath.DegreesToRadians((float)inaccuracyElement.Attribute("min"));
			attributes.MaxInaccuracy = MogreMath.DegreesToRadians((float)inaccuracyElement.Attribute("max"));
			attributes.InaccuracyGrowth = MogreMath.DegreesToRadians((float)inaccuracyElement.Attribute("growth"));

			attributes.MaxRecoil = MogreMath.DegreesToRadians((float)recoilElement.Attribute("max"));
			attributes.RecoilGrowth = MogreMath.DegreesToRadians((float)recoilElement.Attribute("growth"));

			attributes.FirstPersonMuzzleFlashPosition = firstPersonMuzzleFlashElement.ParseXYZ(Vector3.ZERO);

			return attributes;
		}

		private static WeaponAttributes LoadWeaponAttributes(XElement weaponElement)
		{
			XElement attackElement = weaponElement.Element("Attack");
			XElement firstPersonElement = weaponElement.Element("FirstPerson");
			XElement soundElement = weaponElement.Element("Sound");
			return new WeaponAttributes()
			{
				Kind = weaponElement.Attribute("kind").Value.ParseAsEnum<Weapon.Kind>(true),
				Name = weaponElement.Attribute("name").Value,
				ModelName = weaponElement.Attribute("model").Value,
				BaseDamage = (int)attackElement.Attribute("damage"),
				AttackInterval = (float)attackElement.Attribute("interval"),
				AttackSound = soundElement.Attribute("attack").Value,
				FirstPersonPosition = firstPersonElement.ParseXYZ(Vector3.ZERO)
			};
		}

		private struct WeaponAttributes
		{
			public float AttackInterval;
			public string AttackSound;
			public int BaseDamage;
			public Vector3 FirstPersonMuzzleFlashPosition;
			public Vector3 FirstPersonPosition;
			public Weapon.Kind Kind;
			public float InaccuracyGrowth;
			public float MaxInaccuracy;
			public float MaxRecoil;
			public float MinInaccuracy;
			public string ModelName;
			public string Name;
			public float RecoilGrowth;
		}
	}
}

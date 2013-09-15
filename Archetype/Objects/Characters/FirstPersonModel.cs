using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Utilities;

namespace Archetype.Objects.Characters
{
	public class FirstPersonModel : CharacterModel
	{
		private SceneNode EyeNode;

		public FirstPersonModel(Character character, SceneNode eyeNode)
			: base(character, eyeNode)
		{
			this.EyeNode = eyeNode;
		}

		protected override void OnWeaponHandlerChanged()
		{
			base.OnWeaponHandlerChanged();
			if (WeaponEntity != null)
				WeaponSceneNode.Position = Character.Weapon.FirstPersonPosition;
		}

	}
}

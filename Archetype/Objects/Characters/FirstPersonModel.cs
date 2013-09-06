using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.Objects.Characters
{
	public class FirstPersonModel : CharacterModel
	{
		public override bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
				WeaponSceneNode.SetVisible(value);
			}
		}

		private SceneNode EyeNode;
		private SceneNode WeaponSceneNode;
		private bool _visible = true;

		public FirstPersonModel(Character character, SceneNode eyeNode)
			: base(character)
		{
			this.EyeNode = eyeNode;
			WeaponSceneNode = EyeNode.CreateChildSceneNode();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			WeaponSceneNode.Dispose();
		}

		protected override void OnWeaponHandlerChanged()
		{
			base.OnWeaponHandlerChanged();
			if (WeaponEntity != null)
			{
				WeaponSceneNode.AttachObject(WeaponEntity);
				WeaponSceneNode.Position = Character.Weapon.FirstPersonPosition;
			}
		}

	}
}

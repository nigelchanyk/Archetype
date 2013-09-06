using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Logic.WeaponHandlers;

namespace Archetype.Objects.Characters
{
	public abstract class CharacterModel : IDisposable
	{
		public abstract bool Visible { get; set; }

		protected Character Character { get; private set; }
		protected Entity WeaponEntity { get; private set; } // Nullable

		public CharacterModel(Character character)
		{
			this.Character = character;
		}

		public void Dispose()
		{
			OnDispose();
			DisposeWeaponEntity();
		}

		public void WeaponHandlerChanged()
		{
			DisposeWeaponEntity();
			if (Character.ActiveWeaponHandler != null)
				WeaponEntity = Character.World.Scene.CreateEntity(Character.Weapon.ModelName);
			OnWeaponHandlerChanged();

			// Trigger visibility update for newly added entities
			Visible = Visible;
		}

		protected virtual void OnDispose() {}
		protected virtual void OnWeaponHandlerChanged() {}

		private void DisposeWeaponEntity()
		{
			if (WeaponEntity != null)
			{
				WeaponEntity.DetachFromParent();
				WeaponEntity.Dispose();
			}
			WeaponEntity = null;
		}
	}
}

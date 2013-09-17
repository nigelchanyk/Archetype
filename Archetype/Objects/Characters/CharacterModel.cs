using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Events;
using Archetype.Handlers.WeaponHandlers;

namespace Archetype.Objects.Characters
{
	public abstract class CharacterModel : IDisposable
	{
		public virtual bool Visible
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

		protected Character Character { get; private set; }
		protected Entity WeaponEntity { get; private set; } // Nullable
		protected SceneNode WeaponSceneNode { get; set; }
		// For self-centered rotation.
		protected SceneNode WeaponCenterNode { get; set; }

		private bool _visible = true;

		public CharacterModel(Character character)
		{
			this.Character = character;
		}

		public Vector3 ConvertWeaponToWorldPosition(Vector3 position)
		{
			return WeaponCenterNode.ConvertLocalToWorldPosition(position);
		}

		public void Dispose()
		{
			OnDispose();
			DisposeWeaponEntity();
			WeaponCenterNode.Dispose();
			WeaponSceneNode.Dispose();
		}

		public void Update(UpdateEvent evt)
		{
			OnUpdate(evt);
		}

		public void WeaponHandlerChanged()
		{
			DisposeWeaponEntity();
			if (Character.ActiveWeaponHandler != null)
			{
				WeaponEntity = Character.World.Scene.CreateEntity(Character.Weapon.ModelName);
				WeaponCenterNode.AttachObject(WeaponEntity);
			}
			OnWeaponHandlerChanged();

			// Trigger visibility update for newly added entities
			Visible = Visible;
		}

		protected virtual void OnDispose() {}
		protected virtual void OnUpdate(UpdateEvent evt) {}
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Events;
using Archetype.Utilities;

namespace Archetype.Objects.Characters
{
	public class ThirdPersonModel : CharacterModel
	{
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
				BodyNode.SetVisible(value);
			}
		}
		public Entity[] BodyEntities { get; private set; }

		private SceneNode BodyNode;
		private Node WeaponPositionReferenceNode;
		private Vector3 WeaponPositionReferencePosition;

		public ThirdPersonModel(Character character, SceneNode characterNode, string[] bodyEntityNames)
			: base(character, characterNode)
		{
			BodyEntities = bodyEntityNames.Select(name => Character.World.Scene.CreateEntity(name)).ToArray();
			BodyNode = characterNode.CreateChildSceneNode();
			foreach (Entity bodyEntity in BodyEntities)
			{
				BodyNode.AttachObject(bodyEntity);
				// To play multiple animations that does not affect each other,
				// blending mode cannot be average.
				if (bodyEntity.HasSkeleton)
					bodyEntity.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
			}
			WeaponCenterNode.Yaw(MathHelper.Pi);
		}

		protected override void OnDispose()
		{
			base.OnDispose();

			BodyNode.DetachAllObjects();
			BodyNode.Dispose();
			foreach (Entity bodyEntity in BodyEntities)
				bodyEntity.Dispose();
		}

		protected override void OnUpdate(UpdateEvent evt)
		{
			base.OnUpdate(evt);

			if (WeaponPositionReferenceNode != null)
				WeaponSceneNode.Position = WeaponPositionReferenceNode.ConvertLocalToWorldPosition(WeaponPositionReferencePosition);
		}

		protected override void OnWeaponHandlerChanged()
		{
			base.OnWeaponHandlerChanged();
			if (WeaponEntity != null)
			{
				CharacterConfiguration.LocalSpacePosition position = Character.Configuration.GetThirdPersonWeaponPosition(Character.ActiveWeaponHandler.Weapon.Name);
				// Note that the world position of a bone is the entity itself,
				// which is equivalent to the space in character node.
				WeaponPositionReferenceNode = BodyEntities[0].Skeleton.GetBone(position.World);
				WeaponPositionReferencePosition = position.Position;
			}
			else
				WeaponPositionReferenceNode = null;
		}
	}
}

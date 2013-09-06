using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.Objects.Characters
{
	public class ThirdPersonModel : CharacterModel
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
				BodyNode.SetVisible(value);
			}
		}
		public Entity[] BodyEntities { get; private set; }

		private SceneNode BodyNode;
		private bool _visible = true;

		public ThirdPersonModel(Character character, SceneNode characterNode, string[] bodyEntityNames)
			: base(character)
		{
			BodyEntities = bodyEntityNames.Select(name => Character.World.Scene.CreateEntity(name)).ToArray();
			BodyNode = characterNode.CreateChildSceneNode();
			foreach (Entity bodyEntity in BodyEntities)
				BodyNode.AttachObject(bodyEntity);
		}

		protected override void OnDispose()
		{
			BodyNode.DetachAllObjects();
			BodyNode.Dispose();
			foreach (Entity bodyEntity in BodyEntities)
				bodyEntity.Dispose();
		}
	}
}

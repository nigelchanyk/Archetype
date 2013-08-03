using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Assets;
using Archetype.Events;
using Archetype.States;
using Archetype.Utilities;
using Archetype.Logic.ActionHandlers;
using Archetype.Objects.Primitives;

namespace Archetype.Objects.Characters
{
	public abstract class Character : GeneralObject
	{
		public enum LowerBodyAnimationKind
		{
			Idle,
			Walk
		}

		public float EyePitch
		{
			get { return _eyePitch; }
			set
			{
				_eyePitch = value.Clamp(-MathHelper.PiOver3, MathHelper.PiOver3);
				_eyeNode.Orientation = MathHelper.CreateQuaternionFromYawPitchRoll(0, _eyePitch, 0);
			}
		}
		public override Quaternion Orientation
		{
			get { return BodyNode.Orientation; }
			set
			{
				Radian x, y, z;
				value.ToRotationMatrix().ToEulerAnglesXYZ(out x, out y, out z);
				Yaw = x.ValueRadians;
			}
		}
		public override Vector3 Position
		{
			get { return BodyNode.Position; }
			set { BodyNode.Position = value; }
		}
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
				BodyNode.SetVisible(value, true);
			}
		}
		public float Yaw
		{
			get { return _yaw; }
			set
			{
				_yaw = value;
				BodyNode.Orientation = MathHelper.CreateQuaternionFromYawPitchRoll(_yaw, 0, 0);
			}
		}

		protected Entity[] BodyEntities { get; private set; }
		protected PrimitiveNode[] BodyColliders { get; private set; }
		protected SceneNode BodyNode { get; private set; }
		protected SphereNode BoundingSphere { get; private set; }
		protected LowerBodyAnimationKind LowerBodyAnimation { get; set; }
		protected UprightCylinderNode SimpleCollider { get; set; }

		protected abstract JumpHandler JumpHandler { get; set; }
		protected abstract WalkHandler WalkHandler { get; set; }

		private Dictionary<LowerBodyAnimationKind, AnimationState[]> _lowerAnimationMapper = new Dictionary<LowerBodyAnimationKind, AnimationState[]>();
		private float _eyePitch = 0;
		private SceneNode _eyeNode;
		private bool _visible = true;
		private float _yaw = 0;

		public Character(World world, string[] bodyEntityNames, string colliderName)
			: base(world)
		{
			if (bodyEntityNames.Length == 0)
				throw new ArgumentException("At least one body entity must be provided.");

			BodyEntities = bodyEntityNames.Select(name => World.Scene.CreateEntity(name)).ToArray();
			BodyNode = World.WorldNode.CreateChildSceneNode();
			foreach (Entity bodyEntity in BodyEntities)
				BodyNode.AttachObject(bodyEntity);

			BodyColliders = ColliderLoader.ParseColliders(colliderName, BodyEntities[0], "Alpha_").ToArray();

			BuildAnimationMappers();
			BoundingSphere = new SphereNode(BodyNode, new Vector3(0, 1, 0), 2);
			SimpleCollider = new UprightCylinderNode(BodyNode, Vector3.ZERO, 1.7f, 0.4f);
			LowerBodyAnimation = LowerBodyAnimationKind.Idle;
			_eyeNode = BodyNode.CreateChildSceneNode(new Vector3(0, 1.7f, 0));
		}

		public void AttachCamera(Camera camera)
		{
			camera.DetachFromParent();
			_eyeNode.AttachObject(camera);
			camera.Position = Vector3.ZERO;
			camera.Orientation = Quaternion.IDENTITY;
		}

		public float? Intersects(Ray ray)
		{
			// Simple test
			float? result = BoundingSphere.GetIntersectingDistance(ray);
			if (result == null)
				return null;

			// Precise test

			return result;
		}

		public void Jump()
		{
			JumpHandler.Jump();
		}

		public void LookAt(float x, float y, float z)
		{
			LookAt(new Vector3(x, y, z));
		}

		public void LookAt(Vector3 position)
		{
			BodyNode.LookAt(position, Node.TransformSpace.TS_PARENT);
		}

		public void Walk(Vector3 direction)
		{
			WalkHandler.SetNextMoveDirection(direction);
		}

		protected override void OnDispose()
		{
			BodyNode.DetachAllObjects();
			_eyeNode.DetachAllObjects();
			_eyeNode.Dispose();
			foreach (Entity bodyEntity in BodyEntities)
				bodyEntity.Dispose();
			BodyNode.Dispose();
		}

		protected override void OnUpdate(UpdateEvent evt)
		{
			Velocity = WalkHandler.GetVelocityInfluence(evt) + JumpHandler.GetVelocityInfluence(evt);
			MultiAttemptsTranslate(evt.ElapsedTime);

			if (BodyNode.Position.y < 0)
				BodyNode.Position = BodyNode.Position.Mask(true, false, true);
			UpdateAnimation(evt);
		}

		private void BuildAnimationMappers()
		{
			foreach (LowerBodyAnimationKind kind in Enum.GetValues(typeof(LowerBodyAnimationKind)))
			{
				AnimationState[] animation = BodyEntities.Select(entity => entity.GetAnimationState(kind.ToString())).ToArray();
				_lowerAnimationMapper.Add(kind, animation);
			}
		}

		private void UpdateAnimation(UpdateEvent evt)
		{
			foreach (var entry in _lowerAnimationMapper)
			{
				if (entry.Value.Length == 0)
					continue;

				if (entry.Key == LowerBodyAnimation)
				{
					float weight = MathHelper.Lerp(entry.Value[0].Weight, 1, evt.ElapsedTime * 0.01f);
					foreach (AnimationState animationState in entry.Value)
					{
						animationState.Weight = weight;
						animationState.AddTime(evt.ElapsedTime);
					}
				}
				else if (entry.Value.Any(animationState => animationState.Enabled))
				{
					float weight = MathHelper.Lerp(entry.Value[0].Weight, 0, evt.ElapsedTime * 0.01f);
					if (weight < 0.05f)
					{
						foreach (AnimationState animationState in entry.Value)
						{
							animationState.Weight = 0;
							animationState.Enabled = false;
						}
					}
					else
					{
						foreach (AnimationState animationState in entry.Value)
						{
							animationState.Weight = weight;
							animationState.AddTime(evt.ElapsedTime);
						}
					}
				}
			}
		}

		private void MultiAttemptsTranslate(float elapsedTime)
		{
			Vector3 originalPosition = Position;
			BodyNode.Translate(Velocity * elapsedTime, Node.TransformSpace.TS_PARENT);
			BodyNode.InvalidateChildrenCache();
			UprightBoxNode intersectedBuilding = World.GetFirstIntersectingBuilding(SimpleCollider);
			if (intersectedBuilding == null)
				return;

			// Collided with something
			// From here, we work with the building's world to perform smooth partial movements.
			originalPosition = BodyNode.Parent.ConvertToSpace(intersectedBuilding.ReferenceNode, originalPosition);
			Vector3 fullTranslation = BodyNode.ConvertToSpace(intersectedBuilding.ReferenceNode, Vector3.ZERO);
			Vector3 bestDelta = Vector3.ZERO;
			foreach (Vector3 delta in (fullTranslation - originalPosition).CreatePartialVectorCombinations())
			{
				Vector3 currentBestDelta = BinarySearch.Iterate(originalPosition, originalPosition + delta, 3, pos =>
				{
					BodyNode.Position = intersectedBuilding.ReferenceNode.ConvertToSpace(BodyNode.Parent, pos);
					SimpleCollider.InvalidateCache();
					return !World.IntersectBuildings(SimpleCollider);
				}) - originalPosition;

				if (currentBestDelta.SquaredLength > bestDelta.SquaredLength)
					bestDelta = currentBestDelta;
			}

			Position = intersectedBuilding.ReferenceNode.ConvertToSpace(BodyNode.Parent, originalPosition + bestDelta);
		}
	}
}

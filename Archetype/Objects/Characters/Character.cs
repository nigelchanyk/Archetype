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

		public Vector3 EyeHeight { get; private set; }
		public override Quaternion Orientation
		{
			get { return BodyNode.Orientation; }
			set
			{
				BodyNode.Orientation = value;
				Radian x, y, z;
				BodyNode.Orientation.ToRotationMatrix().ToEulerAnglesXYZ(out x, out y, out z);
				_yaw = y.ValueRadians;
				_pitch = x.ValueRadians;
				_roll = z.ValueRadians;
			}
		}
		public float Pitch
		{
			get { return _pitch; }
		}
		public float Roll
		{
			get { return _roll; }
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
		}

		protected Entity[] BodyEntities { get; private set; }
		protected SceneNode BodyNode { get; private set; }
		protected LowerBodyAnimationKind LowerBodyAnimation { get; set; }
		protected UprightCylinderNode SimpleCollider { get; set; }

		protected abstract JumpHandler JumpHandler { get; set; }
		protected abstract WalkHandler WalkHandler { get; set; }

		private Dictionary<LowerBodyAnimationKind, AnimationState[]> _lowerAnimationMapper = new Dictionary<LowerBodyAnimationKind, AnimationState[]>();
		private float _pitch = 0;
		private float _roll = 0;
		private bool _visible = true;
		private float _yaw = 0;

		public Character(World world, string[] bodyEntityNames)
			: base(world)
		{
			BodyEntities = bodyEntityNames.Select(name => World.Scene.CreateEntity(name)).ToArray();
			BodyNode = World.WorldNode.CreateChildSceneNode();
			foreach (Entity bodyEntity in BodyEntities)
				BodyNode.AttachObject(bodyEntity);
			BuildAnimationMappers();
			SimpleCollider = new UprightCylinderNode(BodyNode, Vector3.ZERO, 1.7f, 0.4f);
			LowerBodyAnimation = LowerBodyAnimationKind.Idle;
			EyeHeight = MathHelper.Up * 1.7f;
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

		public void SetYawPitchRoll(float yaw, float pitch, float roll)
		{
			_yaw = yaw;
			_pitch = pitch;
			_roll = roll;
			BodyNode.Orientation = MathHelper.CreateQuaternionFromYawPitchRoll(yaw, pitch, roll);
		}

		public void Walk(Vector3 direction)
		{
			WalkHandler.SetNextMoveDirection(direction);
		}

		protected override void OnDispose()
		{
			SimpleCollider.Dispose();
			BodyNode.DetachAllObjects();
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
			originalPosition = BodyNode.Parent.ConvertToSpace(intersectedBuilding.Node, originalPosition);
			Vector3 fullTranslation = BodyNode.ConvertToSpace(intersectedBuilding.Node, Vector3.ZERO);
			Vector3 bestDelta = Vector3.ZERO;
			foreach (Vector3 delta in (fullTranslation - originalPosition).CreatePartialVectorCombinations())
			{
				Vector3 currentBestDelta = BinarySearch.Iterate(originalPosition, originalPosition + delta, 3, pos =>
				{
					BodyNode.Position = intersectedBuilding.Node.ConvertToSpace(BodyNode.Parent, pos);
					SimpleCollider.InvalidateCache();
					return !World.IntersectBuildings(SimpleCollider);
				}) - originalPosition;

				if (currentBestDelta.SquaredLength > bestDelta.SquaredLength)
					bestDelta = currentBestDelta;
			}

			Position = intersectedBuilding.Node.ConvertToSpace(BodyNode.Parent, originalPosition + bestDelta);
		}
	}
}

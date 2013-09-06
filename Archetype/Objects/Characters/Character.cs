using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Assets;
using Archetype.DataLoaders;
using Archetype.Events;
using Archetype.States;
using Archetype.Utilities;
using Archetype.Logic.ActionHandlers;
using Archetype.Objects.Primitives;
using Archetype.BattleSystems;
using Archetype.Logic.WeaponHandlers;
using Archetype.Objects.Weapons;

namespace Archetype.Objects.Characters
{
	public abstract class Character : GeneralObject
	{
		public enum LowerBodyAnimationKind
		{
			Idle,
			Walk
		}

		public WeaponHandler ActiveWeaponHandler
		{
			get
			{
				return _activeWeaponHandler;
			}
			set
			{
				_activeWeaponHandler = value;
				FirstPersonModel.WeaponHandlerChanged();
				ThirdPersonModel.WeaponHandlerChanged();
			}
		}
		public bool Alive
		{
			get { return Health > 0; }
		}
		public BattlerRecord Record { get; set; }
		public SceneNode EyeNode { get; private set; }
		public float EyePitch
		{
			get { return _eyePitch; }
			set
			{
				_eyePitch = value.Clamp(-MathHelper.PiOver3, MathHelper.PiOver3);
				EyeNode.Orientation = MathHelper.CreateQuaternionFromYawPitchRoll(0, _eyePitch, 0);
			}
		}
		public bool FirstPerson
		{
			get
			{
				return _firstPerson;
			}
			set
			{
				_firstPerson = value;
				FirstPersonModel.Visible = value;
				ThirdPersonModel.Visible = !value;
			}
		}
		public int Health { get; set; }
		public override Quaternion Orientation
		{
			get { return CharacterNode.Orientation; }
			set
			{
				Radian x, y, z;
				value.ToRotationMatrix().ToEulerAnglesXYZ(out x, out y, out z);
				Yaw = x.ValueRadians;
			}
		}
		public override Vector3 Position
		{
			get { return CharacterNode.Position; }
			set { CharacterNode.Position = value; }
		}
		public Weapon Weapon { get { return ActiveWeaponHandler == null ? null : ActiveWeaponHandler.Weapon; } } // Nullable
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
				CharacterNode.SetVisible(value, true);
			}
		}
		public float Yaw
		{
			get { return _yaw; }
			set
			{
				_yaw = value;
				CharacterNode.Orientation = MathHelper.CreateQuaternionFromYawPitchRoll(_yaw, 0, 0);
			}
		}

		protected BodyCollider[] BodyColliders { get; private set; }
		protected SphereNode BoundingSphere { get; private set; }
		protected SceneNode CharacterNode { get; private set; }
		protected LowerBodyAnimationKind LowerBodyAnimation { get; set; }
		protected UprightCylinderNode SimpleCollider { get; set; }

		protected abstract JumpHandler JumpHandler { get; set; }
		protected abstract WalkHandler WalkHandler { get; set; }

		private Dictionary<LowerBodyAnimationKind, AnimationState[]> LowerAnimationMapper = new Dictionary<LowerBodyAnimationKind, AnimationState[]>();
		private FirstPersonModel FirstPersonModel;
		private ThirdPersonModel ThirdPersonModel;
		private WeaponHandler _activeWeaponHandler; // Nullable
		private float _eyePitch = 0;
		private bool _firstPerson = false;
		private bool _visible = true;
		private float _yaw = 0;

		public Character(World world, string[] bodyEntityNames, string colliderName)
			: base(world)
		{
			if (bodyEntityNames.Length == 0)
				throw new ArgumentException("At least one body entity must be provided.");

			CharacterNode = World.WorldNode.CreateChildSceneNode();
			EyeNode = CharacterNode.CreateChildSceneNode(new Vector3(0, 1.7f, 0));
			FirstPersonModel = new FirstPersonModel(this, EyeNode);
			FirstPersonModel.Visible = false;
			ThirdPersonModel = new ThirdPersonModel(this, CharacterNode, bodyEntityNames);

			BodyColliders = ColliderLoader.ParseColliders(colliderName, ThirdPersonModel.BodyEntities[0], "Alpha_").ToArray();

			BuildAnimationMappers();
			BoundingSphere = new SphereNode(CharacterNode, new Vector3(0, 1, 0), 2);
			SimpleCollider = new UprightCylinderNode(CharacterNode, Vector3.ZERO, 1.7f, 0.4f);
			LowerBodyAnimation = LowerBodyAnimationKind.Idle;
			Health = 100;
		}

		public void AttachCamera(Camera camera)
		{
			camera.DetachFromParent();
			EyeNode.AttachObject(camera);
			camera.Position = Vector3.ZERO;
			camera.Orientation = Quaternion.IDENTITY;
		}

		public void Attack(Vector3 eyeSpaceDirection, int baseDamage)
		{
			Ray ray = new Ray(EyeNode.ConvertLocalToWorldPosition(Vector3.ZERO), EyeNode.ConvertLocalToWorldDelta(eyeSpaceDirection));
			BodyCollider collider;
			Character enemy = World.FindEnemy(this, ray, out collider);
			if (enemy != null)
			{
				Console.WriteLine(enemy.Record.Name + " received " + baseDamage * collider.DamageMultiplier + " damage.");
			}
		}

		public float? GetRayCollisionResult(Ray ray, out BodyCollider collider)
		{
			collider = null;
			// Simple test
			float? result = BoundingSphere.GetIntersectingDistance(ray);
			if (result == null)
				return null;

			// Precise test
			float? best = null;
			// Bones consider the scene node itself as world space.
			// Therefore, the given ray must first be converted to the
			// bone's world space.
			ray = ray.TransformRay(CharacterNode);
			foreach (BodyCollider bodyCollider in BodyColliders)
			{
				result = bodyCollider.PrimitiveNode.GetIntersectingDistance(ray);
				if (result == null)
					continue;
				if (best == null || result.Value < best.Value)
				{
					best = result;
					collider = bodyCollider;
				}
			}

			return best;
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
			CharacterNode.LookAt(position, Node.TransformSpace.TS_PARENT);
		}

		public void ReceiveDamage(int damage)
		{
			Health -= damage;
			if (!Alive)
			{
				Visible = false;
				Record.DeathCount++;
			}
		}

		public void RegularAttack()
		{
			if (ActiveWeaponHandler != null)
				ActiveWeaponHandler.RegularAttack();
		}

		public void Walk(Vector3 direction)
		{
			WalkHandler.SetNextMoveDirection(direction);
		}

		protected override void OnDispose()
		{
			CharacterNode.DetachAllObjects();
			EyeNode.DetachAllObjects();
			FirstPersonModel.Dispose();
			ThirdPersonModel.Dispose();
			EyeNode.Dispose();
			CharacterNode.Dispose();
		}

		protected override void OnUpdate(UpdateEvent evt)
		{
			Velocity = WalkHandler.GetVelocityInfluence(evt) + JumpHandler.GetVelocityInfluence(evt);
			MultiAttemptsTranslate(evt.ElapsedTime);
			if (ActiveWeaponHandler != null)
				ActiveWeaponHandler.Update(evt);

			if (CharacterNode.Position.y < 0)
				CharacterNode.Position = CharacterNode.Position.Mask(true, false, true);
			UpdateAnimation(evt);
		}

		private void BuildAnimationMappers()
		{
			foreach (LowerBodyAnimationKind kind in Enum.GetValues(typeof(LowerBodyAnimationKind)))
			{
				AnimationState[] animation = ThirdPersonModel.BodyEntities.Select(entity => entity.GetAnimationState(kind.ToString())).ToArray();
				LowerAnimationMapper.Add(kind, animation);
			}
		}

		private void UpdateAnimation(UpdateEvent evt)
		{
			foreach (var entry in LowerAnimationMapper)
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
			CharacterNode.Translate(Velocity * elapsedTime, Node.TransformSpace.TS_PARENT);
			CharacterNode.InvalidateChildrenCache();
			UprightBoxNode intersectedBuilding = World.GetFirstIntersectingBuilding(SimpleCollider);
			if (intersectedBuilding == null)
				return;

			// Collided with something
			// From here, we work with the building's world to perform smooth partial movements.
			originalPosition = CharacterNode.Parent.ConvertToSpace(intersectedBuilding.ReferenceNode, originalPosition);
			Vector3 fullTranslation = CharacterNode.ConvertToSpace(intersectedBuilding.ReferenceNode, Vector3.ZERO);
			Vector3 bestDelta = Vector3.ZERO;
			foreach (Vector3 delta in (fullTranslation - originalPosition).CreatePartialVectorCombinations())
			{
				Vector3 currentBestDelta = BinarySearch.Iterate(originalPosition, originalPosition + delta, 3, pos =>
				{
					CharacterNode.Position = intersectedBuilding.ReferenceNode.ConvertToSpace(CharacterNode.Parent, pos);
					SimpleCollider.InvalidateCache();
					return !World.IntersectBuildings(SimpleCollider);
				}) - originalPosition;

				if (currentBestDelta.SquaredLength > bestDelta.SquaredLength)
					bestDelta = currentBestDelta;
			}

			Position = intersectedBuilding.ReferenceNode.ConvertToSpace(CharacterNode.Parent, originalPosition + bestDelta);
		}
	}
}

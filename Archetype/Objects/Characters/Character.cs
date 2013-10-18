using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Animation;
using Archetype.Assets;
using Archetype.BattleSystems;
using Archetype.DataLoaders;
using Archetype.Events;
using Archetype.Handlers.ActionHandlers;
using Archetype.Handlers.SpecialMoveHandlers;
using Archetype.Handlers.WeaponHandlers;
using Archetype.Objects.Particles;
using Archetype.Objects.Primitives;
using Archetype.Objects.Weapons;
using Archetype.States;
using Archetype.Utilities;

using Math = System.Math;

namespace Archetype.Objects.Characters
{
	public abstract class Character : GeneralObject
	{
		private enum AnimationKind
		{
			LowerBody,
			UpperBody
		}

		public static readonly string[] AllLowerBodyAnimations =
		{
			"Idle",
			"Walk"
		};

		public static readonly IEnumerable<string> AllUpperBodyAnimations = WeaponLoader.GetWeaponNames().Select(x => "Wield_" + x);

		private static readonly Vector3 AirborneThreshold = new Vector3(0, 0.01f, 0);

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
		public bool Airborne { get; private set; }
		public bool Alive
		{
			get { return Health > 0; }
		}
		public Camera Camera { get; private set; }
		public BattlerRecord Record { get; set; }
		public SceneNode EyeNode { get; private set; }
		public float EyePitch
		{
			get { return _eyePitch; }
			set
			{
				_eyePitch = value.ClampAngle(-MathHelper.PiOver3, MathHelper.PiOver3);
				EyeNode.Orientation = MathHelper.CreateQuaternionFromYawPitchRoll(0, _eyePitch, 0);
			}
		}
		public CharacterConfiguration Configuration { get; private set; }
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
		public bool Movable { get { return MovementLock > 0; } }
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
		public float Recoil
		{
			get
			{
				return _recoil;
			}
			set
			{
				EyePitch += value - _recoil;
				_recoil = value;
			}
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
		public FrustumNode ViewFrustum { get; private set; }
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
		protected UprightCylinderNode SimpleCollider { get; set; }
		protected SpecialMoveHandler[] SpecialMoveHandlers { get; private set; }

		protected abstract JumpHandler JumpHandler { get; set; }
		protected abstract WalkHandler WalkHandler { get; set; }

		private CharacterModel Model { get { return FirstPerson ? (CharacterModel)FirstPersonModel : ThirdPersonModel; } }

		private Dictionary<AnimationKind, AnimationManager> AnimationManagerMapper = new Dictionary<AnimationKind, AnimationManager>();
		private BodyCollisionTree BodyCollisionTree;
		private FirstPersonModel FirstPersonModel;
		private int MovementLock = 1;
		private ThirdPersonModel ThirdPersonModel;
		private WeaponHandler _activeWeaponHandler; // Nullable
		private float _eyePitch = 0;
		private bool _firstPerson = false;
		private float _recoil = 0;
		private bool _visible = true;
		private float _yaw = 0;

		public Character(World world, CharacterConfiguration configuration, string colliderName)
			: base(world)
		{
			this.Configuration = configuration;
			if (Configuration.EntityNames.Length == 0)
				throw new ArgumentException("At least one body entity must be provided.");

			CharacterNode = World.WorldNode.CreateChildSceneNode();
			EyeNode = CharacterNode.CreateChildSceneNode(new Vector3(0, 1.7f, 0));
			FirstPersonModel = new FirstPersonModel(this, EyeNode);
			FirstPersonModel.Visible = false;
			ThirdPersonModel = new ThirdPersonModel(this, CharacterNode, Configuration.EntityNames);

			BodyCollisionTree = new BodyCollisionTree(ThirdPersonModel.BodyEntities[0], AllLowerBodyAnimations.Concat(AllUpperBodyAnimations));
			BodyColliders = ColliderLoader.ParseColliders(colliderName, BodyCollisionTree, "Alpha_").ToArray();

			BoundingSphere = new SphereNode(CharacterNode, new Vector3(0, 1, 0), 2);
			SimpleCollider = new UprightCylinderNode(CharacterNode, Vector3.ZERO, 1.7f, 0.7f);
			AnimationManagerMapper.Add(
				AnimationKind.LowerBody,
				new AnimationManager(
					AllLowerBodyAnimations,
					ThirdPersonModel.BodyEntities,
					"Idle"
				)
			);
			AnimationManagerMapper.Add(
				AnimationKind.UpperBody,
				new AnimationManager(
					AllUpperBodyAnimations,
					ThirdPersonModel.BodyEntities,
					"Wield_USP"
				)
			);
			Camera = World.CreateCamera(Vector3.ZERO, MathHelper.Forward);
			EyeNode.AttachObject(Camera);
			ViewFrustum = new FrustumNode(Camera);

			SpecialMoveHandlers = new SpecialMoveHandler[3];
			Reset();
		}

		public void Attack(Vector3 eyeSpaceDirection, int baseDamage)
		{
			Ray ray = GetEyeRay(eyeSpaceDirection);
			BodyCollider collider;
			Character enemy = World.FindEnemy(this, ray, out collider);
			if (enemy != null)
			{
				enemy.ReceiveDamage(baseDamage * collider.DamageMultiplier);
				Console.WriteLine(enemy.Record.Name + " received " + baseDamage * collider.DamageMultiplier + " damage.");
			}
		}

		public Vector3 ConvertLocalToWorldPosition(Vector3 position)
		{
			return CharacterNode.ConvertLocalToWorldPosition(position);
		}

		public Vector3 ConvertWeaponToWorldPosition(Vector3 position)
		{
			return Model.ConvertWeaponToWorldPosition(position);
		}

		public Vector3 GetDirection()
		{
			return CharacterNode.ConvertLocalToWorldDelta(MathHelper.Forward);
		}

		public Ray GetEyeRay(Vector3 eyeSpaceDirection)
		{
			return new Ray(EyeNode.GetWorldPosition(), EyeNode.ConvertLocalToWorldDelta(eyeSpaceDirection));
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

		public IEnumerable<BodyCollider> GetFrustumCollisionResult(FrustumNode frustum)
		{
			if (!frustum.Intersects(BoundingSphere))
				yield break;

			List<BodyCollider> collidedColliders = new List<BodyCollider>();
			foreach (BodyCollider collider in BodyColliders)
			{
				if (World.IsBodyColliderVisibleFromFrustum(frustum, collider))
					yield return collider;
			}
		}

		public void IncreaseRecoil(float factor, float maxRecoil)
		{
			if (Recoil > maxRecoil)
				return;
			Recoil = Math.Min(Recoil + factor, maxRecoil);
		}

		public void Jump()
		{
			if (!Alive)
				return;
			JumpHandler.Jump();
		}

		public void LockMovement()
		{
			MovementLock--;
			Stop();
		}

		public void LookAt(float x, float y, float z)
		{
			if (!Alive)
				return;
			LookAt(new Vector3(x, y, z));
		}

		public void LookAt(Vector3 position)
		{
			if (!Alive)
				return;
			CharacterNode.LookAt(position, Node.TransformSpace.TS_PARENT);
		}

		public void ReceiveDamage(float damage)
		{
			if (!Alive)
				return;
			Health -= (int)damage;
			if (!Alive)
			{
				Record.NotifyDead();
				Visible = false;
				OnDeath();
			}
		}

		public void Reset()
		{
			Visible = true;
			Health = 100;
			foreach (AnimationManager manager in AnimationManagerMapper.Values)
				manager.CurrentAnimation = manager.DefaultAnimation;
			OnReset();
		}

		public void RegularAttack()
		{
			if (!Alive)
				return;
			if (ActiveWeaponHandler != null)
				ActiveWeaponHandler.RegularAttack();
		}

		public void SpecialMove(int slot)
		{
			if (!Alive)
				return;
			if (SpecialMoveHandlers[slot] == null)
				return;

			SpecialMoveHandlers[slot].Trigger();
		}

		public void Stop()
		{
			if (!WalkHandler.Walking)
				return;

			WalkHandler.Stop();
			AnimationManagerMapper[AnimationKind.LowerBody].CurrentAnimation = "Idle";
		}

		public void UnlockMovement()
		{
			MovementLock++;

			if (MovementLock > 1)
				throw new InvalidOperationException("Unlock more than necessary.");
		}

		public void Walk(Vector3 direction)
		{
			if (!Alive || !Movable)
				return;
			WalkHandler.Direction = direction;
			AnimationManagerMapper[AnimationKind.LowerBody].CurrentAnimation = "Walk";
		}

		protected override void OnDispose()
		{
			Camera.Dispose();
			CharacterNode.DetachAllObjects();
			EyeNode.DetachAllObjects();
			FirstPersonModel.Dispose();
			ThirdPersonModel.Dispose();
			EyeNode.Dispose();
			CharacterNode.Dispose();
		}

		protected override void OnUpdate(UpdateEvent evt)
		{
			Recoil = Math.Max(0, Recoil - evt.ElapsedTime * GameConstants.RecoilReductionFactor);
			Velocity = WalkHandler.GetVelocityInfluence(evt) + JumpHandler.GetVelocityInfluence(evt);
			MultiAttemptsTranslate(evt.ElapsedTime);
			if (ActiveWeaponHandler != null)
				ActiveWeaponHandler.Update(evt);
			foreach (SpecialMoveHandler handler in SpecialMoveHandlers)
			{
				if (handler != null)
					handler.Update(evt);
			}

			if (CharacterNode.Position.y < 0)
				CharacterNode.Position = CharacterNode.Position.Mask(true, false, true);
			Airborne = IsAirborne();

			foreach (AnimationManager manager in AnimationManagerMapper.Values)
				manager.Update(evt);
			BodyCollisionTree.Update(evt);

			// Performed after animation update so that position of the weapon
			// matches the animation.
			Model.Update(evt);
		}

		protected virtual void OnDeath() {}
		protected virtual void OnReset() {}

		private bool IsAirborne()
		{
			Vector3 originalPosition = Position;
			CharacterNode.Translate(-AirborneThreshold, Node.TransformSpace.TS_WORLD);
			CharacterNode.InvalidateChildrenCache();

			try
			{
				if (Position.y <= 0)
					return false;

				UprightBoxNode intersectedBuilding = World.GetFirstIntersectingBuilding(SimpleCollider);
				if (intersectedBuilding == null)
					return true;
			}
			finally
			{
				CharacterNode.Position = originalPosition;
				CharacterNode.InvalidateChildrenCache();
			}

			return false;
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
			CharacterNode.InvalidateChildrenCache();
		}
	}
}

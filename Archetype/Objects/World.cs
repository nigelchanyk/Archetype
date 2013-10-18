﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Assets;
using Archetype.Audio;
using Archetype.BattleSystems;
using Archetype.Cameras;
using Archetype.Events;
using Archetype.Logic;
using Archetype.Objects.Characters;
using Archetype.Objects.Characters.Androids;
using Archetype.Objects.Particles;
using Archetype.Objects.Primitives;
using Archetype.Objects.Projectiles;
using Archetype.States;
using Archetype.Utilities;
using Archetype.Objects.Billboards;
using Archetype.CompoundEffects;

namespace Archetype.Objects
{
	public class World : IDisposable
	{
		public BattleSystem BattleSystem { get; set; }
		public CameraManager CameraManager { get; private set; }
		public bool Paused
		{
			get
			{
				return _paused;
			}
			set
			{
				_paused = value;
				ParticleSystemManager.Paused = _paused;
			}
		}
		public string SceneName { get; private set; }
		public SceneManager Scene { get; private set; }
		public SoundEngine SoundEngine { get; private set; }
		public SceneNode WorldNode { get { return Scene.RootSceneNode; } }

		private List<UprightBoxNode> Buildings = new List<UprightBoxNode>();
		private List<Character> Characters = new List<Character>();
		private BillboardSystemManager BillboardSystemManager;
		private CompoundEffectManager CompoundEffectManager;
		private List<Light> Lights = new List<Light>();
		private UniqueParticleSystemManager ParticleSystemManager;
		private List<Projectile> Projectiles = new List<Projectile>();
		private SearchGraph SearchGraph;
		private bool _paused = false;

		public World(Root root, CameraManager cameraManager, string sceneFile = "")
		{
			this.CameraManager = cameraManager;
			SceneName = sceneFile;
			Scene = root.CreateSceneManager(SceneType.ST_GENERIC);
			SearchGraph = new SearchGraph(this);
			if (sceneFile != "")
			{
				SceneLoader loader = new SceneLoader();
				List<Vector3> PathNodes = new List<Vector3>();
				loader.ParseDotScene(sceneFile, ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, Scene, WorldNode, PathNodes, this);
				Scene.SetSkyDome(true, "SkyBoxes/CloudySky", 5, 8, 2000);
				PathNodes.ForEach(x => SearchGraph.AddVertex(x));
			}
			Scene.AmbientLight = new ColourValue(0.5f, 0.5f, 0.5f);
			Light dirLight = CreateLight(new Vector3(100, 100, 100));
			dirLight.Type = Light.LightTypes.LT_DIRECTIONAL;

			BillboardSystemManager = new Billboards.BillboardSystemManager(Scene, WorldNode);
			ParticleSystemManager = new UniqueParticleSystemManager(Scene, WorldNode);
			CompoundEffectManager = new CompoundEffects.CompoundEffectManager(this);
			SoundEngine = new SoundEngine();
		}

		public void AddBuildingCollisionMesh(UprightBoxNode box)
		{
			Buildings.Add(box);
		}

		public void AddProjectile(Projectile projectile)
		{
			Projectiles.Add(projectile);
		}

		public Camera CreateCamera(Vector3 position, Vector3 lookAt)
		{
			Camera camera = Scene.CreateCamera("Camera" + Guid.NewGuid().ToString());
			camera.Position = position;
			camera.LookAt(lookAt);
			camera.NearClipDistance = 0.1f;
			camera.FarClipDistance = 10000;
			return camera;
		}

		public Character CreateCharacter()
		{
			Character character = new Assaulter(this);
			Characters.Add(character);
			return character;
		}

		public DecayableBillboard CreateDecayableBillboard(BillboardSystemType type, Vector3 position)
		{
			return BillboardSystemManager.CreateBillboard(type, position);
		}

		public DecayableBillboard CreateDecayableBillboard(BillboardSystemType type, Vector3 position, Vector2 dimension, float timeToLive)
		{
			return BillboardSystemManager.CreateBillboard(type, position, dimension, timeToLive);
		}

		public void CreateMuzzleFlashEffect(Character character, Vector3 weaponSpacePosition)
		{
			CompoundEffectManager.CreateMuzzleFlashEffect(character, weaponSpacePosition);
		}

		public ParticleEmitterCluster CreateParticleEmitterCluster(ParticleSystemType type, Vector3 position, bool eternal)
		{
			return ParticleSystemManager.CreateParticleEmitterCluster(type, position, eternal);
		}

		public void DestroyCharacter(Character character)
		{
			Characters.Remove(character);
			character.Dispose();
		}

		public Light CreateLight(float x, float y, float z)
		{
			return CreateLight(new Vector3(x, y, z));
		}

		public Light CreateLight(Vector3 position)
		{
			Light light = Scene.CreateLight();
			light.Position = position;
			Lights.Add(light);
			return light;
		}

		public void Dispose()
		{
			Characters.ForEach(character => character.Dispose());
			CompoundEffectManager.Dispose();
			BillboardSystemManager.Dispose();
			ParticleSystemManager.Dispose();
			WorldNode.Dispose();
			Scene.DestroyAllAnimations();
			Scene.DestroyAllAnimationStates();
			Scene.DestroyAllManualObjects();
			Scene.DestroyAllCameras();
			Scene.DestroyAllEntities();
			Scene.DestroyAllLights();
			Scene.DestroyAllMovableObjects();
			Scene.DestroyAllParticleSystems();
			Scene.Dispose();

			SoundEngine.Dispose();
		}

		public void DestroyLight(Light light)
		{
			Scene.DestroyLight(light);
			Lights.Remove(light);
		}

		public UprightBoxNode GetFirstIntersectingBuilding(UprightCylinderNode cylinder)
		{
			return Buildings.FirstOrDefault(building => building.Intersects(cylinder));
		}

		/// <summary>
		/// Return the shortest intersecting building distance. (IMPORTANT: Ray must be in world space!!!)
		/// </summary>
		/// <param name="ray">Ray in world space</param>
		/// <returns></returns>
		public float? GetNearestIntersectingBuilding(Ray ray)
		{
			return Buildings.MinOrNull(building => building.GetIntersectingDistance(ray));
		}

		public bool IntersectBuildings(UprightCylinderNode cylinder)
		{
			return GetFirstIntersectingBuilding(cylinder) != null;
		}

		public bool IsBodyColliderVisibleFromFrustum(FrustumNode frustum, BodyCollider collider)
		{
			Vector3 colliderCenter = collider.PrimitiveNode.GetCenter(true);
			if (!frustum.Contains(colliderCenter))
				return false;

			Vector3 delta = colliderCenter - frustum.Position;
			Ray ray = new Ray(frustum.Position, delta);
			float? buildingDistance = GetNearestIntersectingBuilding(ray);
			if (!buildingDistance.HasValue || delta.SquaredLength < buildingDistance.Value.Squared())
				return true;

			return false;
		}

		public bool IsValidPath(Vector3 source, Vector3 dest)
		{
			Ray path = new Ray(source, dest - source);
			return Buildings.All(x => x.GetIntersectingDistance(path) == null);
		}

		public Character FindEnemy(Character attacker, Ray ray, out BodyCollider collider)
		{
			float? intersection;
			return FindEnemy(attacker, ray, out collider, out intersection);
		}

		public Character FindEnemy(Character attacker, Ray ray, out BodyCollider collider, out float? intersection)
		{
			collider = null;
			if (BattleSystem == null)
			{
				intersection = Buildings.MinOrNull(x => x.GetIntersectingDistance(ray));
				return null;
			}
			float? characterIntersection = null;
			Character closest = null;
			foreach (Character enemy in BattleSystem.GetEnemiesAlive(attacker))
			{
				float? tempIntersection = enemy.GetRayCollisionResult(ray, out collider);
				if (tempIntersection == null)
					continue;
				if (characterIntersection == null || tempIntersection.Value < characterIntersection.Value)
				{
					characterIntersection = tempIntersection;
					closest = enemy;
				}
			}
			intersection = characterIntersection;
			if (characterIntersection == null)
				return null;

			// Determine if any buildings block the ray
			float? buildingIntersection = Buildings.MinOrNull(x => x.GetIntersectingDistance(ray));
			if (buildingIntersection != null && buildingIntersection.Value < characterIntersection.Value)
			{
				intersection = buildingIntersection;
				return null;
			}

			return closest;
		}

		public Path FindPath(Vector3 source, Vector3 destination)
		{
			return SearchGraph.Search(source, destination);
		}

		public Vector3[] GetAdjacentVertices(Vector3 source)
		{
			return SearchGraph.GetAdjacentVertices(source);
		}

		public Vector3? GetClosestVertex(Vector3 source)
		{
			return SearchGraph.GetClosestVertex(source);
		}

		public void Reset()
		{
			Projectiles.ForEach(x => x.Dispose());
			Projectiles.Clear();
		}

		public void Update(UpdateEvent evt)
		{
			Characters.ForEach(character => character.Update(evt));
			Projectiles.ForEach(projectile => projectile.Update(evt));
			Projectiles.RemoveAll(projectile => 
			{
				if (!projectile.Alive)
					projectile.Dispose();
				return !projectile.Alive;
			});
			BillboardSystemManager.Update(evt);
			ParticleSystemManager.Update(evt);
			// Compound Effect Manager should be updated only after all managers completed their updates.
			// Some compound effects are dependent on the `Alive` properties of other managers' objects.
			// Otherwise, it will take two cycles to destroy a compound effect.
			CompoundEffectManager.Update(evt);
			SoundEngine.SetListenerPosition(CameraManager.ActiveCamera.RealPosition, CameraManager.ActiveCamera.RealDirection);
		}
	}
}

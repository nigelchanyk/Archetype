using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

using Archetype.Objects.Characters;
using Archetype.Objects.Characters.Androids;
using Archetype.States;
using Archetype.Utilities;
using Archetype.Events;
using Archetype.Objects.Primitives;

namespace Archetype.Objects
{
	public class World : IDisposable
	{
		public Camera Camera { get; private set; }
		public SceneManager Scene { get; private set; }
		public SceneNode WorldNode { get { return Scene.RootSceneNode; } }

		private List<UprightBoxNode> _buildings = new List<UprightBoxNode>();
		private List<Character> _characters = new List<Character>();
		private List<Light> _lights = new List<Light>();

		public World(Root root, string sceneFile)
		{
			Scene = root.CreateSceneManager(SceneType.ST_GENERIC);
			SceneLoader loader = new SceneLoader();
			loader.ParseDotScene(sceneFile, ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, Scene, WorldNode, this);
			Scene.AmbientLight = new ColourValue(0.5f, 0.5f, 0.5f);
			InitializeCamera(new Vector3(0, 0, -5), Vector3.ZERO);
			Light dirLight = CreateLight(new Vector3(100, 100, 100));
			dirLight.Type = Light.LightTypes.LT_DIRECTIONAL;
		}

		public void AddBuildingCollisionMesh(UprightBoxNode box)
		{
			_buildings.Add(box);
		}

		public Character CreateCharacter()
		{
			Character character = new Assaulter(this);
			_characters.Add(character);
			return character;
		}

		public void DestroyCharacter(Character character)
		{
			_characters.Remove(character);
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
			_lights.Add(light);
			return light;
		}

		public void Dispose()
		{
			_characters.ForEach(character => character.Dispose());
			WorldNode.Dispose();
			Scene.DestroyAllAnimations();
			Scene.DestroyAllAnimationStates();
			Scene.DestroyAllManualObjects();
			Scene.DestroyAllCameras();
			Scene.DestroyAllEntities();
			Scene.DestroyAllLights();
			Scene.DestroyAllMovableObjects();
			Scene.DestroyAllParticleSystems();
		}

		public void DestroyLight(Light light)
		{
			Scene.DestroyLight(light);
			_lights.Remove(light);
		}

		public UprightBoxNode GetFirstIntersectingBuilding(UprightCylinderNode cylinder)
		{
			return _buildings.FirstOrDefault(building => building.Intersects(cylinder));
		}

		/// <summary>
		/// Return the shortest intersecting building distance. (IMPORTANT: Ray must be in world space!!!)
		/// </summary>
		/// <param name="ray">Ray in world space</param>
		/// <returns></returns>
		public float? GetShortestIntersectingBuildingDistance(Ray ray)
		{
			return _buildings.MinOrNull(building => building.GetIntersectingDistance(ray));
		}

		public void InflictDamage(Character attacker, Ray ray, int baseDamage)
		{
			float? shortestIntersectingBuildingDistance = GetShortestIntersectingBuildingDistance(ray);
		}

		public bool IntersectBuildings(UprightCylinderNode cylinder)
		{
			return GetFirstIntersectingBuilding(cylinder) != null;
		}

		public void Update(UpdateEvent evt)
		{
			_characters.ForEach(character => character.Update(evt));
		}

		private void InitializeCamera(Vector3 position, Vector3 lookAt)
		{
			Camera = Scene.CreateCamera("Camera");
			Camera.Position = position;
			Camera.LookAt(lookAt);
			Camera.NearClipDistance = 0.1f;
			Camera.FarClipDistance = 500;
		}
	}
}

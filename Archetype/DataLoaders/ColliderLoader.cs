using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Mogre;

using Archetype.Objects.Primitives;
using Archetype.Utilities;
using Archetype.Objects.Characters;

namespace Archetype.DataLoaders
{
	public static class ColliderLoader
	{
		private static Dictionary<string, Armature> ColliderCache = new Dictionary<String, Armature>();

		/// <summary>
		/// Match bones with colliders and create primitive nodes.
		/// </summary>
		/// <param name="colliderConfiguration">Filename of the collision configuration</param>
		/// <param name="entity">Entity (model) loaded</param>
		/// <param name="armaturePrefix">Prefix of the armature name within the model</param>
		/// <returns>A list of primitive nodes created</returns>
		public static List<BodyCollider> ParseColliders(string colliderConfiguration, Entity entity, string armaturePrefix)
		{
			Armature armature = ColliderCache.GetOrCreate(colliderConfiguration);
			List<BodyCollider> result = new List<BodyCollider>();
			RecursiveCreate(armature, entity.Skeleton, armaturePrefix, result);
			return result;
		}

		private static Armature GetOrCreate(this Dictionary<string, Armature> cache, string colliderConfiguration)
		{
			if (cache.ContainsKey(colliderConfiguration))
				return cache[colliderConfiguration];

			XElement root = XElement.Load("Assets/Data/Colliders/" + colliderConfiguration);
			Armature rootArmature = new Armature(root);
			cache.Add(colliderConfiguration, rootArmature);
			return rootArmature;
		}

		private static void RecursiveCreate(Armature armature, Skeleton skeleton, string prefix, List<BodyCollider> result)
		{
			foreach (ColliderTemplate collider in armature.Colliders)
				collider.InsertCollider(skeleton.GetBone(prefix + armature.Name), result);
			foreach (Armature child in armature.Children)
				RecursiveCreate(child, skeleton, prefix, result);
		}


		private class Armature
		{
			public string Name { get; private set; }
			public ColliderTemplate[] Colliders { get; private set; }
			public Armature[] Children { get; private set; }
			
			public Armature(XElement element)
			{
				element = element.Element("Armature");
				Name = element.Attribute("name").Value;
				var collidersEnumerator = element.Elements("Collider");
				Colliders = new ColliderTemplate[collidersEnumerator.Count()];
				int i = 0;
				foreach (XElement colliderElement in collidersEnumerator)
					Colliders[i++] = CreateColliderTemplate(colliderElement);

				var childEnumerator = element.Elements("Armature");
				Children = new Armature[childEnumerator.Count()];
				i = 0;
				foreach (XElement childElement in childEnumerator)
					Children[i++] = new Armature(childElement);
			}

			private static ColliderTemplate CreateColliderTemplate(XElement element)
			{
				switch (element.Attribute("type").Value)
				{
					case "box":
						return new BoxTemplate(element);
					case "sphere":
						return new SphereTemplate(element);
				}

				throw new ArgumentException("Unknown collider type: " + element.Attribute("type").Value);
			}
		}


		private abstract class ColliderTemplate
		{
			public float DamageMultipler { get; private set; }
			public Vector3 Position { get; private set; }

			public ColliderTemplate(XElement element)
			{
				Position = element.ParseXYZ(Vector3.ZERO);
				DamageMultipler = (float)element.Attribute("damage");
			}

			public abstract void InsertCollider(Node node, List<BodyCollider> result);
		}


		private class BoxTemplate : ColliderTemplate
		{
			public Vector3 Max { get; private set; }
			public Vector3 Min { get; private set; }

			public BoxTemplate(XElement element)
				: base(element)
			{
				Min = element.Element("Min").ParseXYZ(Vector3.ZERO);
				Max = element.Element("Max").ParseXYZ(Vector3.ZERO);
			}

			public override void InsertCollider(Node node, List<BodyCollider> result)
			{
				result.Add(new BodyCollider(new BoxNode(node, Min, Max), DamageMultipler));
			}
		}


		private class SphereTemplate : ColliderTemplate
		{
			public float Radius { get; private set; }

			public SphereTemplate(XElement element)
				: base(element)
			{
				Radius = (float)element.Attribute("radius");
			}

			public override void InsertCollider(Node node, List<BodyCollider> result)
			{
				result.Add(new BodyCollider(new SphereNode(node, Position, Radius), DamageMultipler));
			}
		}
	}
}

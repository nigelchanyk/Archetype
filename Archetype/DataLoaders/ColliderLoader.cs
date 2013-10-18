using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Mogre;

using Archetype.Utilities;
using Archetype.Objects.Characters;
using Archetype.Objects.Primitives;

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
		public static List<BodyCollider> ParseColliders(string colliderConfiguration, BodyCollisionTree tree, string armaturePrefix)
		{
			Armature armature = ColliderCache.GetOrCreate(colliderConfiguration, tree);
			List<BodyCollider> result = new List<BodyCollider>();
			RecursiveCreate(armature, tree, armaturePrefix, result);
			return result;
		}

		private static Armature GetOrCreate(this Dictionary<string, Armature> cache, string colliderConfiguration, BodyCollisionTree tree)
		{
			if (cache.ContainsKey(colliderConfiguration))
				return cache[colliderConfiguration];

			XElement root = XElement.Load("Assets/Data/Colliders/" + colliderConfiguration);
			Armature rootArmature = Armature.FromRoot(root, tree);
			cache.Add(colliderConfiguration, rootArmature);
			return rootArmature;
		}

		private static void RecursiveCreate(Armature armature, BodyCollisionTree tree, string prefix, List<BodyCollider> result)
		{
			foreach (ColliderTemplate collider in armature.Colliders)
				collider.InsertCollider(tree.GetNode(prefix + armature.Name), result);
			foreach (Armature child in armature.Children)
				RecursiveCreate(child, tree, prefix, result);
		}


		private class Armature
		{
			public static Armature FromRoot(XElement root, BodyCollisionTree tree)
			{
				return new Armature(root.Element("Armature"), tree);
			}

			public string Name { get; private set; }
			public ColliderTemplate[] Colliders { get; private set; }
			public Armature[] Children { get; private set; }
			
			private Armature(XElement element, BodyCollisionTree tree)
			{
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
					Children[i++] = new Armature(childElement, tree);
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

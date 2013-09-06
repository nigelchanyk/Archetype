using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Events;
using Archetype.Utilities;

namespace Archetype.Objects.Particles
{
	public class UniqueParticleSystem : IDisposable
	{
		public bool Paused
		{
			get { return _paused; }
			set
			{
				_paused = value;
				if (_paused)
					ParticleSystem.SpeedFactor = 0;
				else
					ParticleSystem.SpeedFactor = 1;
			}
		}

		private HashSet<ParticleEmitterCluster> ClusterSet = new HashSet<ParticleEmitterCluster>();
		private uint ParticleQuotaPerEmitter;
		private ParticleSystem ParticleSystem;
		private ParticleEmitter[] ReferenceEmitters;
		private SceneNode SceneNode;
		private bool _paused = false;

		public UniqueParticleSystem(SceneManager sceneManager, SceneNode worldNode, string templateName)
		{
			ParticleSystem = sceneManager.CreateParticleSystem("UniqueParticleSystem" + Guid.NewGuid().ToString(), templateName);
			ReferenceEmitters = ParticleSystem.GetParticleEmitterEnumerable().ToArray();
			foreach (ParticleEmitter emitter in ReferenceEmitters)
				emitter.Enabled = false;
			ParticleQuotaPerEmitter = ParticleSystem.ParticleQuota;

			SceneNode = worldNode.CreateChildSceneNode();
			SceneNode.AttachObject(ParticleSystem);
		}

		public ParticleEmitterCluster CreateParticleEmitterCluster(Vector3 position)
		{
			ParticleEmitterCluster cluster = new ParticleEmitterCluster(ParticleSystem, ReferenceEmitters, position, ParticleQuotaPerEmitter);
			ClusterSet.Add(cluster);
			return cluster;
		}

		public void Dispose()
		{
			SceneNode.DetachAllObjects();
			ParticleSystem.Dispose();
			SceneNode.Dispose();
		}

		public void Update(UpdateEvent evt)
		{
			foreach (ParticleEmitterCluster cluster in ClusterSet)
				cluster.Update(evt);
			ClusterSet.RemoveWhere(x => !x.Alive);
		}
	}
}

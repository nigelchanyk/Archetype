using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Utilities;
using Archetype.Events;

namespace Archetype.Objects.Particles
{
	public class UniqueParticleSystemManager : IDisposable
	{
		public bool Paused
		{
			get { return _paused; }
			set
			{
				_paused = value;
				foreach (UniqueParticleSystem particleSystem in ParticleSystemMapper.Values)
					particleSystem.Paused = value;
			}
		}

		private Dictionary<ParticleSystemType, UniqueParticleSystem> ParticleSystemMapper = new Dictionary<ParticleSystemType, UniqueParticleSystem>();
		private bool _paused = false;

		public UniqueParticleSystemManager(SceneManager sceneManager, SceneNode worldNode)
		{
			foreach (ParticleSystemType type in EnumHelper.GetValues<ParticleSystemType>())
				ParticleSystemMapper.Add(type, new UniqueParticleSystem(sceneManager, worldNode, type.ToString()));
		}

		public ParticleEmitterCluster CreateParticleEmitterCluster(ParticleSystemType type, Vector3 position, bool eternal)
		{
			return ParticleSystemMapper[type].CreateParticleEmitterCluster(position, eternal);
		}

		public void Dispose()
		{
			foreach (UniqueParticleSystem particleSystem in ParticleSystemMapper.Values)
				particleSystem.Dispose();
		}

		public void Update(UpdateEvent evt)
		{
			foreach (UniqueParticleSystem particleSystem in ParticleSystemMapper.Values)
				particleSystem.Update(evt);
		}
	}
}

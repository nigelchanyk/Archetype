using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Events;
using Archetype.Utilities;

using Math = System.Math;

namespace Archetype.Objects.Particles
{
	public class ParticleEmitterCluster
	{
		public bool Alive
		{
			get { return TimeToLive > 0; }
		}
		/// <summary>
		/// If true, emitters will not stop until stop() is called.
		/// </summary>
		public bool Eternal { get; private set; }
		public Vector3 Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
				if (!Alive)
					return;

				foreach (ParticleEmitter emitter in Emitters)
					emitter.Position = value;
			}
		}

		private bool Disposed = false;
		private ParticleEmitter[] Emitters;
		private uint ParticleQuotaPerEmitter;
		private ParticleSystem ParticleSystem;
		private float TimeToLive;
		private Vector3 _position;

		public ParticleEmitterCluster(ParticleSystem particleSystem, ParticleEmitter[] referenceEmitters, Vector3 position, uint particleQuotaPerEmitter, bool eternal)
		{
			this.ParticleSystem = particleSystem;
			this.ParticleQuotaPerEmitter = particleQuotaPerEmitter;
			this.Eternal = eternal;

			Emitters = new ParticleEmitter[referenceEmitters.Length];
			for (int i = 0; i < referenceEmitters.Length; ++i)
			{
				Emitters[i] = ParticleSystem.AddEmitter(referenceEmitters[i].Type);
				referenceEmitters[i].CopyParametersTo(Emitters[i]);
				TimeToLive = Math.Max(TimeToLive, Emitters[i].MaxTimeToLive + Emitters[i].MaxDuration);
			}
			this.Position = position;

			ParticleSystem.ParticleQuota += ParticleQuotaPerEmitter;
		}

		public void Stop()
		{
			TimeToLive = 0;
			Dispose();
		}

		public void Update(UpdateEvent evt)
		{
			if (!Eternal)
				TimeToLive = Math.Max(0, TimeToLive - evt.ElapsedTime);
			Dispose();
		}

		private void Dispose()
		{
			if (!Alive && !Disposed)
			{
				// A very unfortunate O(n^2) approach because the person who wrote
				// ParticleSystem::RemoveEmitter did not think this through.
				foreach (ParticleEmitter emitter in Emitters)
					ParticleSystem.RemoveEmitter(emitter);

				ParticleSystem.ParticleQuota -= ParticleQuotaPerEmitter;
				Disposed = true;
			}
		}
	}
}

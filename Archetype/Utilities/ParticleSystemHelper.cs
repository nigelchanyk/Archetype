using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Archetype.Utilities
{
	public static class ParticleSystemHelper
	{
		public static IEnumerable<ParticleEmitter> GetParticleEmitterEnumerable(this ParticleSystem particleSystem)
		{
			for (ushort i = 0; i < particleSystem.NumEmitters; ++i)
				yield return particleSystem.GetEmitter(i);
		}

		public static void RemoveEmitter(this ParticleSystem particleSystem, ParticleEmitter emitter)
		{
			for (ushort i = 0; i < particleSystem.NumEmitters; ++i)
			{
				if (particleSystem.GetEmitter(i) == emitter)
				{
					particleSystem.RemoveEmitter(i);
					return;
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Assets;
using Archetype.DataLoaders;
using Archetype.Objects.Particles;
using Archetype.Utilities;

namespace Archetype.Objects.Characters.Androids
{
	public abstract class Android : Character
	{
		public Android(World world)
			: base(world, CharacterConfigurationLoader.Get("Alpha"), AssetCollections.AlphaColliders)
		{

		}

		protected override void OnDeath()
		{
			Visible = false;
			World.CreateParticleEmitterCluster(ParticleSystemType.Explosion, EyeNode.GetWorldPosition(), false);
			World.SoundEngine.Play3D("Assets/Audio/Explosions/Android.ogg", Position);
		}
	}
}

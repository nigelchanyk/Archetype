﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Archetype.Assets;
using Archetype.Objects.Particles;
using Archetype.Utilities;

namespace Archetype.Objects.Characters.Androids
{
	public abstract class Android : Character
	{
		private static readonly String[] MaleBodyEntityNames =
		{
			AssetCollections.AlphaJoints,
			AssetCollections.AlphaLimbs,
			AssetCollections.AlphaTorso
		};

		public Android(World world)
			: base(world, MaleBodyEntityNames, AssetCollections.AlphaColliders)
		{

		}

		protected override void OnDeath()
		{
			Visible = false;
			World.CreateParticleEmitterCluster(ParticleSystemType.Explosion, EyeNode.GetWorldPosition());
			World.SoundEngine.Play3D("Assets/Audio/Explosions/Android.ogg", Position);
		}
	}
}

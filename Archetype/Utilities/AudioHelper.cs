using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IrrKlang;
using Mogre;

namespace Archetype.Utilities
{
	public static class AudioHelper
	{
		public static ISound Play3D(this ISoundEngine soundEngine, string soundFileName, Vector3 position, bool loop = false)
		{
			return soundEngine.Play3D(soundFileName, position.x, position.y, position.z, loop);
		}

		public static void SetListenerPosition(this ISoundEngine soundEngine, Camera camera)
		{
			Vector3 position = camera.RealPosition;
			Vector3 direction = camera.RealDirection;
			soundEngine.SetListenerPosition(position.x, position.y, position.z, direction.x, direction.y, direction.z);
		}
	}
}

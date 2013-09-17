using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype
{
	public static class GameConstants
	{
		public static readonly float BotYawLerpAmount = 3;
		public static readonly float BotYawSpeed = Mogre.Math.DegreesToRadians(720);
		public static readonly ushort CursorZOrder = ushort.MaxValue;
		public static readonly float DefaultGravityAcceleration = -9.8f;
		public static readonly float DefaultWalkingSpeed = 3;
		public static readonly int InaccuracyReductionFactor = 5;
		public static readonly float RecoilReductionFactor = 0.1f;
		public static readonly float MaxRecoil = Mogre.Math.DegreesToRadians(10);
	}
}

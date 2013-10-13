using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Objects.Characters;
using Archetype.Objects.Projectiles;
using Archetype.Utilities;

namespace Archetype.Handlers.SpecialMoveHandlers
{
	public class PlasmaBeamHandler : SpecialMoveHandler
	{
		private static readonly int PlasmaBeamBaseDamage = 75;
		private static readonly float PlasmaBeamInterval = 1;
		private static readonly float PlasmaBeamSpeed = 100;
		private static readonly float PlasmaTimeToLive = 2;

		public PlasmaBeamHandler(Character actionPerformer)
			: base(actionPerformer, PlasmaBeamInterval)
		{
		}

		protected override void OnTrigger()
		{
			Ray ray = ActionPerformer.GetEyeRay(MathHelper.Forward);
			ActionPerformer.World.AddProjectile(new PlasmaBeam(
				ActionPerformer.World,
				ActionPerformer,
				ray.GetPoint(0.3f),
				ray.Direction * PlasmaBeamSpeed,
				PlasmaBeamBaseDamage,
				PlasmaTimeToLive
			));
			ActionPerformer.World.SoundEngine.Play3D("Assets/Audio/SpecialMoves/PlasmaBeam.ogg", ray.Origin);
		}
	}
}

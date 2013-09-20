using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archetype.Objects.Characters;

namespace Archetype.Controllers.BotControllers.Strategies
{
	public abstract class EnemyInteractionStrategy : Strategy
	{
		public Character Enemy { get; protected set; }
		public BodyCollider VisibleCollider { get; protected set; }

		public EnemyInteractionStrategy(BotController controller, Character enemy, BodyCollider visibleCollider)
			: base(controller)
		{
		}

		protected bool IsEnemyVisible()
		{
			if (BotController.Character.World.IsBodyColliderVisibleFromFrustum(BotController.Character.ViewFrustum, VisibleCollider))
				return true;

			VisibleCollider = BotController.SelectBestCollider(Enemy);
			return VisibleCollider != null;
		}
	}
}

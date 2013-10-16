﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archetype.Objects.Characters;
using Mogre;
using Archetype.Events;
using Archetype.Utilities;

namespace Archetype.Controllers.BotControllers.Strategies
{
	/// <summary>
	/// An enemy disappeared. Wait for enemy to appear again.
	/// (In this period of time, the reaction delay is 0.)
	/// </summary>
	public class WaitStrategy : Strategy
	{
		private float ElapsedTime = 0;
		private Vector3 LastSeen;

		public WaitStrategy(BotController controller, Vector3 lastSeen)
			: base(controller)
		{
			this.LastSeen = lastSeen;
		}

		public override Strategy NextStrategy()
		{
			Strategy next = base.NextStrategy();
			if (next != this)
				return next;

			BodyCollider visibleCollider;
			Character enemy = BotController.GetVisibleEnemyBodyCollider(out visibleCollider);
			if (enemy != null)
				return new AttackStrategy(BotController, enemy, visibleCollider);
			if (ElapsedTime < BotController.WaitDelay)
				return this;
			if (MathHelper.Randomizer.NextFloat() <= BotController.AggressionFactor)
				return new PursueStrategy(BotController, LastSeen);

			return new RoamStrategy(BotController);
		}

		public override void Update(UpdateEvent evt)
		{
			ElapsedTime += evt.ElapsedTime;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Events;

namespace Archetype.Objects.Billboards
{
	public class DecayableBillboard
	{
		public bool Alive { get; private set; }
		public Vector2 Dimension
		{
			get { return new Vector2(Billboard.OwnWidth, Billboard.OwnHeight); }
			set { Billboard.SetDimensions(value.x, value.y); }
		}
		public Vector3 Position
		{
			get { return Billboard.Position; }
			set { Billboard.SetPosition(value.x, value.y, value.z); }
		}
		public float TimeToLive { get; private set; }

		private Billboard Billboard;
		private BillboardSet BillboardSet;

		public DecayableBillboard(BillboardSet billboardSet, Vector3 position, Vector2 dimension, float timeToLive)
		{
			this.BillboardSet = billboardSet;
			Billboard = BillboardSet.CreateBillboard(position);
			Billboard.TexcoordRect = new FloatRect(0, 0, 1, 1);
			this.Dimension = dimension;
			this.TimeToLive = timeToLive;
			this.Alive = true;
		}

		public void Update(UpdateEvent evt)
		{
			if (TimeToLive <= 0)
				return;

			TimeToLive -= evt.ElapsedTime;
			if (TimeToLive <= 0)
			{
				Alive = false;
				BillboardSet.RemoveBillboard(Billboard);
			}
		}
	}
}

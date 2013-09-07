using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Archetype.Events;

namespace Archetype.Objects.Billboards
{
	public class BillboardSystemManager : IDisposable
	{
		private Dictionary<BillboardSystemType, BillboardSystem> BillboardSystemMapper = new Dictionary<BillboardSystemType, BillboardSystem>();

		public BillboardSystemManager(SceneManager sceneManager, SceneNode worldNode)
		{
			BillboardSystemMapper.Add(BillboardSystemType.MuzzleFlash, new BillboardSystem(sceneManager, worldNode, "Billboards/MuzzleFlash", BillboardType.BBT_POINT, new Vector2(0.3f, 0.3f), 0.05f));
		}

		public DecayableBillboard CreateBillboard(BillboardSystemType type, Vector3 position)
		{
			return BillboardSystemMapper[type].CreateBillboard(position);
		}

		public DecayableBillboard CreateBillboard(BillboardSystemType type, Vector3 position, Vector2 dimension, float timeToLive)
		{
			return BillboardSystemMapper[type].CreateBillboard(position, dimension, timeToLive);
		}

		public void Dispose()
		{
			foreach (BillboardSystem billboardSystem in BillboardSystemMapper.Values)
				billboardSystem.Dispose();
		}

		public void Update(UpdateEvent evt)
		{
			foreach (BillboardSystem billboardSystem in BillboardSystemMapper.Values)
				billboardSystem.Update(evt);
		}
	}
}

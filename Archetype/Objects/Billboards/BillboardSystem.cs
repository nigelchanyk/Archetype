using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using Archetype.Events;

namespace Archetype.Objects.Billboards
{
	public class BillboardSystem : IDisposable
	{
		public Vector2 DefaultDimension { get; set; }
		public float DefaultTimeToLive { get; set; }

		private BillboardSet BillboardSet;
		private HashSet<DecayableBillboard> DecayableBillboardSet = new HashSet<DecayableBillboard>();

		public BillboardSystem(SceneManager sceneManager, SceneNode worldNode, string materialName, Mogre.BillboardType type, Vector2 defaultDimension, float defaultTimeToLive)
		{
			BillboardSet = sceneManager.CreateBillboardSet();
			BillboardSet.SetMaterialName(materialName);
			BillboardSet.SetBillboardsInWorldSpace(true);
			BillboardSet.BillboardType = type;
			worldNode.AttachObject(BillboardSet);
			this.DefaultDimension = defaultDimension;
			this.DefaultTimeToLive = defaultTimeToLive;
		}

		public DecayableBillboard CreateBillboard(Vector3 position)
		{
			return CreateBillboard(position, DefaultDimension, DefaultTimeToLive);
		}

		public DecayableBillboard CreateBillboard(Vector3 position, Vector2 dimension, float timeToLive)
		{
			DecayableBillboard billboard = new DecayableBillboard(BillboardSet, position, dimension, timeToLive);
			DecayableBillboardSet.Add(billboard);
			return billboard;
		}

		public void Dispose()
		{
			BillboardSet.Clear();
			BillboardSet.DetachFromParent();
			BillboardSet.Dispose();
		}

		public void Update(UpdateEvent evt)
		{
			foreach (DecayableBillboard billboard in DecayableBillboardSet)
				billboard.Update(evt);

			DecayableBillboardSet.RemoveWhere(x => !x.Alive);
		}
	}
}

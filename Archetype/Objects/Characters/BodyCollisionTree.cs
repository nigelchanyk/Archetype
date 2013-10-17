using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using MogreAnimation = Mogre.Animation;

namespace Archetype.Objects.Characters
{
	public class BodyCollisionTree
	{
		private MirrorNode Root;
		private SceneNode Parent;

		public BodyCollisionTree(Entity bodyEntity)
		{
			Parent = bodyEntity.ParentSceneNode;
			Root = new MirrorNode(Parent, bodyEntity.Skeleton.GetBoneIterator().First(x => x.Parent == null));
		}

		private class MirrorNode
		{
			public string Name { get { return ReferenceBone.Name; } }
			public Node Node { get; private set; }
			public Bone ReferenceBone { get; private set; }

			private List<MirrorNode> Children;

			public MirrorNode(Node parentNode, Bone referenceBone)
			{
				this.ReferenceBone = referenceBone;
				Node = parentNode.CreateChild(ReferenceBone.InitialPosition, ReferenceBone.InitialOrientation);
				Children = new List<MirrorNode>(ReferenceBone.NumChildren());
				foreach (Bone bone in referenceBone.GetChildIterator().OfType<Bone>())
					Children.Add(new MirrorNode(Node, bone));
			}

			public void ApplyAnimation(MogreAnimation animation, AnimationState animationState)
			{
				Node.Position = ReferenceBone.InitialPosition;
				Node.Orientation = ReferenceBone.InitialOrientation;
				if (!animation.HasNodeTrack(ReferenceBone.Handle))
					return;

				TimeIndex index = new TimeIndex(animationState.TimePosition);
				TransformKeyFrame keyFrame = new TransformKeyFrame(null, 0);
				animation.GetNodeTrack(ReferenceBone.Handle).GetInterpolatedKeyFrame(index, keyFrame);
				Node.Position += keyFrame.Translate * animationState.Weight;
				Node.Orientation *= Quaternion.Slerp(animationState.Weight, Quaternion.IDENTITY, keyFrame.Rotation);
			}
		}
	}
}

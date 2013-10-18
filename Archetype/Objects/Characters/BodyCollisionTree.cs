using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using MogreAnimation = Mogre.Animation;
using Archetype.Events;

namespace Archetype.Objects.Characters
{
	public class BodyCollisionTree
	{
		private List<MogreAnimation> Animations = new List<MogreAnimation>();
		private List<AnimationState> AnimationStates = new List<AnimationState>();
		private Dictionary<string, MirrorNode> MirrorMapper = new Dictionary<string, MirrorNode>();
		private MirrorNode Root;
		private SceneNode Parent;

		public BodyCollisionTree(Entity bodyEntity, IEnumerable<string> animationNames)
		{
			Parent = bodyEntity.ParentSceneNode;
			Root = new MirrorNode(Parent, bodyEntity.Skeleton.GetBoneIterator().First(x => x.Parent == null));
			foreach (string name in animationNames.Where(x => bodyEntity.Skeleton.HasAnimation(x)))
			{
				Animations.Add(bodyEntity.Skeleton.GetAnimation(name));
				AnimationStates.Add(bodyEntity.GetAnimationState(name));
			}

			foreach (MirrorNode node in Root.GetEnumerable())
				MirrorMapper.Add(node.Name, node);
		}

		public Node GetNode(string boneName)
		{
			return MirrorMapper[boneName].Node;
		}

		public void Update(UpdateEvent evt)
		{
			Root.ResetAnimation();
			for (int i = 0; i < Animations.Count; ++i)
			{
				if (!AnimationStates[i].Enabled)
					continue;
				Root.AddAnimation(Animations[i], AnimationStates[i]);
			}
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

			public void AddAnimation(MogreAnimation animation, AnimationState animationState)
			{
				Children.ForEach(x => x.AddAnimation(animation, animationState));

				if (!animation.HasNodeTrack(ReferenceBone.Handle))
					return;

				TimeIndex index = new TimeIndex(animationState.TimePosition);
				TransformKeyFrame keyFrame = new TransformKeyFrame(null, 0);
				animation.GetNodeTrack(ReferenceBone.Handle).GetInterpolatedKeyFrame(index, keyFrame);
				Node.Position += keyFrame.Translate * animationState.Weight;
				Node.Orientation *= Quaternion.Slerp(animationState.Weight, Quaternion.IDENTITY, keyFrame.Rotation);
			}

			public IEnumerable<MirrorNode> GetEnumerable()
			{
				yield return this;
				foreach (IEnumerable<MirrorNode> childEnumerable in Children.Select(x => x.GetEnumerable()))
				{
					foreach (MirrorNode child in childEnumerable)
						yield return child;
				}
			}

			public void ResetAnimation()
			{
				Node.Position = ReferenceBone.InitialPosition;
				Node.Orientation = ReferenceBone.InitialOrientation;
				Children.ForEach(x => x.ResetAnimation());
			}
		}
	}
}

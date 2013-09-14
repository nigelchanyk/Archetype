using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Archetype.Events;
using Archetype.Utilities;

namespace Archetype.Animation
{
	public class AnimationManager
	{
		public string CurrentAnimation
		{
			get
			{
				return _currentAnimation;
			}
			set
			{
				if (!HasAnimation(value))
					throw new NotSupportedException("Animation " + value + " not defined.");
				_currentAnimation = value;
			}
		}

		private Dictionary<string, AnimationState[]> AnimationMapper = new Dictionary<string, AnimationState[]>();
		private Entity[] Entities;
		private string _currentAnimation;

		public AnimationManager(IEnumerable<string> animationNames, Entity[] entities, string defaultAnimation)
		{
			this.Entities = entities;
			BuildAnimationMapper(animationNames);
			CurrentAnimation = defaultAnimation;
		}

		public bool HasAnimation(string animation)
		{
			return AnimationMapper.ContainsKey(animation);
		}

		public void Update(UpdateEvent evt)
		{
			foreach (var entry in AnimationMapper)
			{
				if (entry.Value.Length == 0)
					continue;

				if (entry.Key == CurrentAnimation)
				{
					float weight = MathHelper.Lerp(entry.Value[0].Weight, 1, evt.ElapsedTime * 0.01f);
					foreach (AnimationState animationState in entry.Value)
					{
						animationState.Weight = weight;
						animationState.Enabled = true;
						animationState.AddTime(evt.ElapsedTime);
					}
				}
				else if (entry.Value.Any(animationState => animationState.Enabled))
				{
					float weight = MathHelper.Lerp(entry.Value[0].Weight, 0, evt.ElapsedTime * 0.01f);
					if (weight < 0.05f && weight > 0)
					{
						foreach (AnimationState animationState in entry.Value)
						{
							animationState.Weight = 0;
							animationState.TimePosition = 0;
							animationState.Enabled = false;
						}
					}
					else
					{
						foreach (AnimationState animationState in entry.Value)
						{
							animationState.Weight = weight;
							animationState.AddTime(evt.ElapsedTime);
						}
					}
				}
			}
		}

		private void BuildAnimationMapper(IEnumerable<string> animationNames)
		{
			foreach (string animationName in animationNames)
			{
				if (!Entities.All(entity => entity.AllAnimationStates.HasAnimationState(animationName)))
					continue;

				AnimationState[] animation = Entities.Select(entity => entity.GetAnimationState(animationName)).ToArray();
				AnimationMapper.Add(animationName, animation);
			}
		}
	}
}

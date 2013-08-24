using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype.Utilities
{
	public class Bijection<T, U>
	{
		private Dictionary<T, U> LeftMapper = new Dictionary<T, U>();
		private Dictionary<U, T> RightMapper = new Dictionary<U, T>();

		public void Add(T left, U right)
		{
			LeftMapper.Add(left, right);
			RightMapper.Add(right, left);
		}

		public U this[T key]
		{
			get { return LeftMapper[key]; }
		}

		public T this[U key]
		{
			get { return RightMapper[key]; }
		}

		public bool Contains(T key)
		{
			return LeftMapper.ContainsKey(key);
		}

		public bool Contains(U key)
		{
			return RightMapper.ContainsKey(key);
		}
	}
}

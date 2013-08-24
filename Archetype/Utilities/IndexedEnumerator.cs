using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype.Utilities
{
	public class IndexedEnumerator<T> : IDisposable
	{

		public int Index { get; private set; }
		public T Item
		{
			get { return Enumerator.Current; }
		}

		private IEnumerator<T> Enumerator;
		private bool HasValue;

		public IndexedEnumerator(IEnumerable<T> enumerable)
		{
			Enumerator = enumerable.GetEnumerator();
			HasValue = Enumerator.MoveNext();
		}

		public void Dispose()
		{
			Enumerator.Dispose();
		}

		public bool HasNext()
		{
			return HasValue;
		}

		public void Next()
		{
			Enumerator.MoveNext();
			Index++;
		}
	}
}

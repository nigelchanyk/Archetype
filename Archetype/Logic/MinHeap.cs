using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archetype.Logic
{
	public class MinHeap
	{
		public int Count { get; private set; }

		private KeyValuePair<float, int>[] h;

		public MinHeap(int maxSize)
		{
			Count = 0;
			h = new KeyValuePair<float, int>[maxSize];
		}

		public bool ContainsValue(int value)
		{
			for (int i = 0; i < Count; ++i)
			{
				if (h[i].Value == value)
					return true;
			}

			return false;
		}

		public void Add(float key, int value)
		{
			Count++;
			h[Count - 1] = new KeyValuePair<float, int>(key, value);
			SwapUp(Count - 1);
		}

		public KeyValuePair<float, int> Poll()
		{
			if (Count == 0)
				throw new InvalidOperationException("Poll operation cannot be performed on an empty heap.");

			Swap(0, Count - 1);
			Count--;
			Heapify(0);
			return h[Count];
		}

		public void Update(float key, int value)
		{
			for (int i = 0; i < Count; ++i)
			{
				if (h[i].Value == value)
				{
					bool smaller = key < h[i].Key;
					h[i] = new KeyValuePair<float, int>(key, value);
					if (smaller)
						SwapUp(i);
					else
						Heapify(i);
					return;
				}
			}

			throw new KeyNotFoundException("Yeah... Can't find your value " + value);
		}

		private void SwapUp(int index)
		{
			int i = index;
			while (i > 0 && h[i].Key < h[Parent(i)].Key)
			{
				Swap(i, Parent(i));
				i = Parent(i);
			}
		}

		private void Heapify(int index)
		{
			int leftIndex = Left(index);
			int rightIndex = Right(index);

			if (leftIndex >= Count && rightIndex >= Count)
				return;

			if (rightIndex >= Count)
			{
				if (h[index].Key > h[leftIndex].Key)
				{
					Swap(index, leftIndex);
					Heapify(leftIndex);
				}
			}
			else if (h[leftIndex].Key < h[index].Key && h[leftIndex].Key < h[rightIndex].Key)
			{
				Swap(index, leftIndex);
				Heapify(leftIndex);
			}
			else if (h[rightIndex].Key < h[index].Key && h[rightIndex].Key < h[leftIndex].Key)
			{
				Swap(index, rightIndex);
				Heapify(rightIndex);
			}
		}

		private int Parent(int index)
		{
			return (index - 1) / 2;
		}

		private int Left(int index)
		{
			return 2 * index + 1;
		}

		private int Right(int index)
		{
			return 2 * index + 2;
		}

		private void Swap(int index1, int index2)
		{
			KeyValuePair<float, int> temp = h[index1];
			h[index1] = h[index2];
			h[index2] = temp;
		}
	}
}

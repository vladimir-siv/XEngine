using System;

namespace XEngine.Structures
{
	internal sealed class Queue<TValue>
	{
		private readonly Node<TValue>.Pooler Nodes = new Node<TValue>.Pooler();

		private Node<TValue> First = null;
		private Node<TValue> Last = null;
		public int Count { get; private set; } = 0;

		public void Enqueue(TValue value)
		{
			var node = Nodes.Create(value);
			if (Count == 0) First = Last = node;
			else Last = Last.Next = node;
			++Count;
		}

		public TValue Dequeue()
		{
			if (Count == 0) throw new InvalidOperationException("Collection is empty.");
			--Count;
			var node = First;
			First = First.Next;
			if (Count == 0) Last = null; // [redundant]
			Nodes.Release(node);
			return node.Value;
		}

		public TValue Peek()
		{
			if (Count == 0) throw new InvalidOperationException("Collection is empty.");
			return First.Value;
		}

		public TValue Second()
		{
			if (Count < 2) throw new InvalidOperationException("Collection is empty or the second element does not exist.");
			return First.Next.Value;
		}

		public void Clear()
		{
			while (Count > 0) Dequeue();
		}
	}
}

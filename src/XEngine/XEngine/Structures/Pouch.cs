using System.Collections.Generic;

namespace XEngine.Structures
{
	internal sealed class Pouch<TKey, TValue>
	{
		private readonly Node<TValue>.Pooler Nodes = new Node<TValue>.Pooler();
		
		private readonly Dictionary<TKey, Node<TValue>> Collection = new Dictionary<TKey, Node<TValue>>();
		public int Count { get; private set; } = 0;
		
		public void Add(TKey key, TValue value)
		{
			var found = Collection.TryGetValue(key, out var node);
			if (found) node.Next = Nodes.Create(value, node.Next);
			else Collection.Add(key, Nodes.Create(value));
			++Count;
		}
		public bool Retrieve(TKey key, out TValue value)
		{
			var found = Collection.TryGetValue(key, out var node);

			if (!found)
			{
				value = default;
				return false;
			}

			if (node.Next != null)
			{
				var temp = node.Next;
				node.Next = temp.Next;
				node = temp;
			}
			else Collection.Remove(key);

			--Count;
			value = node.Value;
			Nodes.Release(node);
			return true;
		}
	}
}

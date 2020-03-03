using System.Collections.Generic;

namespace XEngine.Structures
{
	// If ValueTypes are needed, it is possible to remove the generic constraints, but 4 poolers have to be used
	// Keys dictionary can be removed, but then, each node from l1 to l3 has to contain it's key with itself, but in that case, multiple poolers have to be used

	internal sealed class Pouch3L<TKey1, TKey2, TKey3, TValue>
		where TKey1 : class
		where TKey2 : class
		where TKey3 : class
		where TValue : class
	{
		private readonly Node<object>.Pooler Nodes = new Node<object>.Pooler();

		private readonly Dictionary<TKey1, Node<object>> RandomAccessL1 = new Dictionary<TKey1, Node<object>>();
		private readonly Dictionary<(TKey1, TKey2), Node<object>> RandomAccessL2 = new Dictionary<(TKey1, TKey2), Node<object>>();
		private readonly Dictionary<(TKey1, TKey2, TKey3), Node<object>> RandomAccessL3 = new Dictionary<(TKey1, TKey2, TKey3), Node<object>>();
		private readonly Dictionary<Node<object>, (TKey1, TKey2, TKey3)> Keys = new Dictionary<Node<object>, (TKey1, TKey2, TKey3)>();

		private Node<object> Head = null;
		public int Count { get; private set; } = 0;

		public void Add(TKey1 key1, TKey2 key2, TKey3 key3, TValue value)
		{
			++Count;

			var l4 = Nodes.Create(value);
			if (RandomAccessL3.TryGetValue((key1, key2, key3), out var node))
			{
				l4.Next = (Node<object>)node.Value;
				node.Value = l4;
				return;
			}

			var l3 = Nodes.Create(l4);
			RandomAccessL3.Add((key1, key2, key3), l3);
			Keys.Add(l3, (key1, key2, key3));

			if (RandomAccessL2.TryGetValue((key1, key2), out node))
			{
				l3.Next = (Node<object>)node.Value;
				node.Value = l3;
				return;
			}

			var l2 = Nodes.Create(l3);
			RandomAccessL2.Add((key1, key2), l2);
			Keys.Add(l2, (key1, key2, null));

			if (RandomAccessL1.TryGetValue(key1, out node))
			{
				l2.Next = (Node<object>)node.Value;
				node.Value = l2;
				return;
			}

			Head = Nodes.Create(l2, Head);
			RandomAccessL1.Add(key1, Head);
			Keys.Add(Head, (key1, null, null));
		}
		public IEnumerable<TValue> Retrieve()
		{
			for (var i1 = Head; i1 != null; i1 = i1.Next)
			{
				for (var i2 = (Node<object>)i1.Value; i2 != null; i2 = i2.Next)
				{
					for (var i3 = (Node<object>)i2.Value; i3 != null; i3 = i3.Next)
					{
						for (var i4 = (Node<object>)i3.Value; i4 != null; i4 = i4.Next)
						{
							yield return (TValue)i4.Value;
						}
					}
				}
			}
		}
		public IEnumerable<TValue> Recover()
		{
			var temp = (Node<object>)null;
			var key = ((TKey1)null, (TKey2)null, (TKey3)null);

			for (var i1 = Head; i1 != null; )
			{
				for (var i2 = (Node<object>)i1.Value; i2 != null; )
				{
					for (var i3 = (Node<object>)i2.Value; i3 != null; )
					{
						for (var i4 = (Node<object>)i3.Value; i4 != null; )
						{
							yield return (TValue)i4.Value;

							temp = i4;
							i4 = i4.Next;
							Nodes.Release(temp);
						}

						key = Keys[i3];
						Keys.Remove(i3);
						RandomAccessL3.Remove((key.Item1, key.Item2, key.Item3));

						temp = i3;
						i3 = i3.Next;
						Nodes.Release(temp);
					}

					key = Keys[i2];
					Keys.Remove(i2);
					RandomAccessL2.Remove((key.Item1, key.Item2));

					temp = i2;
					i2 = i2.Next;
					Nodes.Release(temp);
				}

				key = Keys[i1];
				Keys.Remove(i1);
				RandomAccessL1.Remove(key.Item1);

				temp = i1;
				i1 = i1.Next;
				Nodes.Release(temp);
			}

			Head = null;
			Count = 0;
		}
		public IEnumerable<TValue> Redeem(bool last) => last ? Recover() : Retrieve();
	}
}

using System.Collections.Generic;

namespace XEngine.Structures
{
	internal class Node<TValue>
	{
		public class Pooler
		{
			private readonly Stack<Node<TValue>> Pool = new Stack<Node<TValue>>();
			public void Release(Node<TValue> node) => Pool.Push(node);
			public Node<TValue> Create(TValue value, Node<TValue> next = null)
			{
				Node<TValue> node;
				if (Pool.Count > 0) node = Pool.Pop();
				else node = new Node<TValue>();
				node.Value = value;
				node.Next = next;
				return node;
			}
		}

		public TValue Value;
		public Node<TValue> Next;
		public Node(TValue value, Node<TValue> next = null) { Value = value; Next = next; }
		private Node() { }
	}
}

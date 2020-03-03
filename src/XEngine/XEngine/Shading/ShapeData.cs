using System;

using GlmNet;

namespace XEngine.Shading
{
	public struct ShapeData
	{
		public static vertex[] Positions3f(params float[] positions)
		{
			var vertices = new vertex[positions.Length / 3];
			
			for (var i = 0; i < vertices.Length; ++i)
			{
				vertices[i] = new vertex
				(
					new vec3
					(
						positions[3 * i + 0],
						positions[3 * i + 1],
						positions[3 * i + 2]
					)
				);
			}

			return vertices;
		}

		public vertex[] Vertices { get; private set; }
		public int[] Indices { get; private set; }

		public float[] SerializeData(VertexAttribute attributes)
		{
			if (attributes == VertexAttribute.NONE) throw new ArgumentException("Cannot serialize shape data with no selected attributes.");

			var vertexsize = vertex.SizeOf(attributes);
			var data = new float[Vertices.Length * vertexsize];

			for (int i = 0; i < Vertices.Length; ++i)
			{
				var vertex = Vertices[i];
				var c = 0;

				if (attributes.HasFlag(VertexAttribute.POSITION))
				{
					data[i * vertexsize + c++] = vertex.position.x;
					data[i * vertexsize + c++] = vertex.position.y;
					data[i * vertexsize + c++] = vertex.position.z;
				}

				if (attributes.HasFlag(VertexAttribute.COLOR))
				{
					data[i * vertexsize + c++] = vertex.color.x;
					data[i * vertexsize + c++] = vertex.color.y;
					data[i * vertexsize + c++] = vertex.color.z;
				}

				if (attributes.HasFlag(VertexAttribute.NORMAL))
				{
					data[i * vertexsize + c++] = vertex.normal.x;
					data[i * vertexsize + c++] = vertex.normal.y;
					data[i * vertexsize + c++] = vertex.normal.z;
				}

				if (attributes.HasFlag(VertexAttribute.UV))
				{
					data[i * vertexsize + c++] = vertex.uv.x;
					data[i * vertexsize + c++] = vertex.uv.y;
				}
			}

			return data;
		}

		public ShapeData(vertex[] vertices, int[] indices = null)
		{
			Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));

			if (indices == null)
			{
				indices = new int[Vertices.Length];

				for (var i = 0; i < indices.Length; ++i)
				{
					indices[i] = i;
				}
			}
			
			Indices = indices;
		}

		internal void Release()
		{
			Vertices = null;
			Indices = null;
		}
	}
}

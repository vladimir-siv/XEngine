using System;
using SharpGL;

namespace XEngine.Shading
{
	using XEngine.Common;

	public class GeometricShape : IDisposable
	{
		private ShapeData _ShapeData;
		protected ShapeData ShapeData => _ShapeData;

		public uint AttribCount { get; private set; } = vertex.AttribCount;
		private void UpdateAttribCount() => AttribCount = Math.Min(((uint)Attributes).BitCount(), vertex.AttribCount);
		private VertexAttribute _Attributes = VertexAttribute.ALL;
		public VertexAttribute Attributes
		{
			get { return _Attributes; }
			set { if (_Attributes == value) return; _Attributes = value; UpdateAttribCount(); }
		}

		public vertex[] Vertices => ShapeData.Vertices;
		public int[] Indices => ShapeData.Indices;
		public float[] Data => ShapeData.SerializeData(Attributes);

		public int VertexCount { get; }
		public int IndexCount { get; }

		public bool KeepAlive { get; set; } = false;

		internal GeometricShape(ShapeData shapeData) { _ShapeData = shapeData; VertexCount = Vertices.Length; IndexCount = Indices.Length; }
		public virtual uint OpenGLShapeType => OpenGL.GL_TRIANGLES;

		public GeometricShape Use(VertexAttribute attributes)
		{
			Attributes = attributes;
			return this;
		}

		public virtual VertexAttribute GetAttribute(uint index)
		{
			for (var i = 0u; i < vertex.AttribCount; ++i)
			{
				var e = (VertexAttribute)(1 << (int)i);

				if (Attributes.HasFlag(e))
				{
					if (index-- == 0u) return e;
				}
			}

			return VertexAttribute.NONE;
		}

		public virtual uint GetAttribType(VertexAttribute attribute) => OpenGL.GL_FLOAT;
		public virtual bool ShouldAttribNormalize(VertexAttribute attribute) => false;

		public void Dispose() => Dispose(false);
		public void Dispose(bool force)
		{
			if (KeepAlive && !force) return;
			_ShapeData.Release();
		}
	}
}

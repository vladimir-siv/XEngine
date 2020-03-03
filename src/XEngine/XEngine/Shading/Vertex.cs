using System;
using GlmNet;

namespace XEngine.Shading
{
	public struct vertex
	{
		public static int SizeOf(VertexAttribute attributes)
		{
			var size = 0;

			for (var i = 0u; i < AttribCount; ++i)
			{
				var e = (VertexAttribute)(1 << (int)i);
				if (attributes.HasFlag(e)) size += AttribSize(e);
			}

			return size;
		}
		public static int AttribSize(int i) => AttribSize((VertexAttribute)(1 << i));
		public static int AttribSize(VertexAttribute attribute)
		{
			switch (attribute)
			{
				case VertexAttribute.POSITION: return 3;
				case VertexAttribute.COLOR: return 3;
				case VertexAttribute.NORMAL: return 3;
				case VertexAttribute.UV: return 2;
				default: return 0;
			}
		}
		public const uint AttribCount = 4u;

		public const uint TotalSize = 11u;
		public const uint TotalByteSize = TotalSize * sizeof(float);

		public vec3 position { get; set; }
		public vec3 color { get; set; }
		public vec3 normal { get; set; }
		public vec2 uv { get; set; }

		public vertex(vec3 position) : this(position, Color.Black.rgb) { }
		public vertex(vec3 position, vec3 color) : this(position, color, new vec3(+0.0f, +0.0f, +0.0f)) { }
		public vertex(vec3 position, vec3 color, vec3 normal) : this(position, color, normal, new vec2(0.0f, 0.0f)) { }
		public vertex(vec3 position, vec3 color, vec3 normal, vec2 uv)
		{
			this.position = position;
			this.color = color;
			this.normal = normal;
			this.uv = uv;
		}

		public override string ToString() => $"[{position.x},{position.y},{position.z}:{color.x},{color.y},{color.z}:{normal.x},{normal.y},{normal.z}:{uv.x},{uv.y}]";
	}

	[Flags] public enum VertexAttribute
	{
		NONE = 0,
		POSITION = 1,
		COLOR = 2,
		NORMAL = 4,
		UV = 8,
		ALL = 0x7FFFFFFF
	}
}

using GlmNet;

namespace XEngine.Shading
{
	using XEngine.Common;

	public struct Color
	{
		public static readonly Color White = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		public static readonly Color Black = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		public static readonly Color Gray = new Color(0.5f, 0.5f, 0.5f, 1.0f);
		public static readonly Color DeepSky = new Color(0.529f, 0.808f, 0.922f, 1.0f);

		public static Color FromBytes(byte r, byte g, byte b, byte a = 255) => new Color(r / 255.0f, g / 255.0f, b / 255.0f);

		public vec4 vectorized;

		public float r
		{
			get => vectorized.x;
			set => vectorized.x = value;
		}
		public float g
		{
			get => vectorized.y;
			set => vectorized.y = value;
		}
		public float b
		{
			get => vectorized.z;
			set => vectorized.z = value;
		}
		public float a
		{
			get => vectorized.w;
			set => vectorized.w = value;
		}

		public vec3 rgb => vectorized.to_vec3(false);

		public Color(vec3 rgb) : this(rgb.x, rgb.y, rgb.z) { }
		public Color(float r, float g, float b) : this(r, g, b, 1.0f) { }
		public Color(float r, float g, float b, float a) : this(new vec4(r, g, b, a)) { }
		public Color(vec4 rgba) { vectorized = rgba; }

		public static Color operator +(Color c1, Color c2) => new Color(c1.vectorized + c2.vectorized);
		public static Color operator *(Color c, float v) => new Color(c.vectorized * v);
		public static Color operator *(float v, Color c) => new Color(c.vectorized * v);

		public static bool AreEqual(Color c1, Color c2) => c1.vectorized == c2.vectorized;
	}
}

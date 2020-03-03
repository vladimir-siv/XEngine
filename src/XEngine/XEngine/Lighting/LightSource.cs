using GlmNet;

namespace XEngine.Lighting
{
	using XEngine.Shading;
	using XEngine.Common;

	public struct LightSource
	{
		public static readonly LightSource PitchBlack = new LightSource(vector3.zero, Color.Black, 0.0f, Attenuation.None);
		public static readonly LightSource Sun = new LightSource(new vec3(100.0f, 500.0f, 25.0f), Color.White, 1.0f, Attenuation.None, true) { name = "Sun" };

		public static LightSource Point(vec3 position) => Point(position, Color.White);
		public static LightSource Point(vec3 position, Color color) => Point(position, color, 60.0f);
		public static LightSource Point(vec3 position, Color color, float power) => new LightSource(position, color, power, Attenuation.Default);

		public static LightSource Directional(vec3 position) => Directional(position, Color.White);
		public static LightSource Directional(vec3 position, Color color) => Directional(position, color, 1.0f);
		public static LightSource Directional(vec3 position, Color color, float power) => new LightSource(position, color, power, Attenuation.None);

		public string name;
		public vec3 position;
		public Color color;
		public float power;
		public Attenuation attenuation;
		public bool important;

		public LightSource(float x, float y, float z) : this(new vec3(x, y, z)) { }
		public LightSource(vec3 position) : this(position, Color.White) { }
		public LightSource(vec3 position, Color color) : this(position, color, 60.0f) { }
		public LightSource(vec3 position, Color color, float power) : this(position, color, power, Attenuation.Default) { }
		public LightSource(vec3 position, Color color, float power, Attenuation attenuation) : this(position, color, power, attenuation, false) { }
		public LightSource(vec3 position, Color color, float power, Attenuation attenuation, bool important)
		{
			name = null;
			this.position = position;
			this.color = color;
			this.power = power;
			this.attenuation = attenuation;
			this.important = important;
		}

		public static bool AreEqual(LightSource ls1, LightSource ls2) =>
			ls1.position == ls2.position
			&&
			Color.AreEqual(ls1.color, ls2.color)
			&&
			ls1.power == ls2.power
			&&
			Attenuation.AreEqual(ls1.attenuation, ls2.attenuation);

		public static float operator ^(LightSource ls, vec3 pos)
		{
			var distance = vector3.distance(ls.position, pos);
			if (ls.important) return -1000000.0f / distance;
			else return distance;
		}
	}
}

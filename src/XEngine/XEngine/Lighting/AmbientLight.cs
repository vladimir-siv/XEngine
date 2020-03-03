namespace XEngine.Lighting
{
	using XEngine.Shading;

	public struct AmbientLight
	{
		public static readonly AmbientLight Bright = new AmbientLight(Color.White, 25.0f);

		public Color color;
		public float power;

		public AmbientLight(Color color, float power)
		{
			this.color = color;
			this.power = power / 100f;
		}

		public static bool AreEqual(AmbientLight al1, AmbientLight al2) => Color.AreEqual(al1.color, al2.color) && al1.power == al2.power;
	}
}

namespace XEngine.Lighting
{
	public struct Attenuation
	{
		public static readonly Attenuation None = new Attenuation(1.0f, 0.0f, 0.0f);
		public static readonly Attenuation Default = new Attenuation(1.05f, 0.0f, 0.05f);

		public float x0;
		public float x1;
		public float x2;

		public Attenuation(float x0, float x1, float x2)
		{
			this.x0 = x0;
			this.x1 = x1;
			this.x2 = x2;
		}

		public static bool AreEqual(Attenuation a1, Attenuation a2) => a1.x0 == a2.x0 && a1.x1 == a2.x1 && a1.x2 == a2.x2;
	}
}

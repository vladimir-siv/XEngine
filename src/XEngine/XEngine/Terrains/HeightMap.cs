using System;

namespace XEngine.Terrains
{
	public abstract class HeightMap
	{
		private uint _Granularity = 0u;
		public uint Granularity
		{
			get { return _Granularity; }
			protected set { if (value == 0u) throw new ArgumentException("Granularity cannot be zero."); _Granularity = value; }
		}

		public abstract float GetHeight(float x, float z);
	}
}

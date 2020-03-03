using System;

namespace XEngine.Terrains
{
	using XEngine.Common;

	public class ProceduralHeightMap : HeightMap
	{
		private const int W1 = 49632;
		private const int W2 = 325176;

		public float Amplitude { get; set; } = 75.0f;
		public uint Octaves { get; set; } = 3u;
		public float Roughness { get; set; } = 0.25f;

		private float[,] Noise = null;

		public ProceduralHeightMap(uint granularity, bool generateNoise = true)
		{
			Granularity = granularity;
			if (generateNoise) Generate(RNG.Int(1000000000));
		}
		public ProceduralHeightMap(uint granularity, int w1, int w2) : this(granularity, RNG.Int(1000000000), w1, w2) { }
		public ProceduralHeightMap(uint granularity, int seed) : this(granularity, seed, W1, W2) { }
		public ProceduralHeightMap(uint granularity, int seed, int w1, int w2)
		{
			Granularity = granularity;

			Generate(seed, w1, w2);
		}

		public void Generate(int seed, int w1 = W1, int w2 = W2)
		{
			var s = (long)w1* Granularity + (long)w2 * seed;

			var rng = new Random((int)s);

			var length = Granularity + 1u;

			Noise = new float[length, length];

			for (var x = 0; x < length; ++x)
			{
				for (var z = 0; z < length; ++z)
				{
					Noise[x, z] = (float)(rng.NextDouble() * 2.0 - 1.0);
				}
			}
		}

		public override float GetHeight(float x, float z)
		{
			var height = 0.0f;

			var freq = (float)Math.Pow(2.0, Octaves);
			var rough = 1.0f / Roughness;

			for (var i = 0; i < Octaves; ++i)
			{
				freq /= 2.0f;
				rough *= Roughness;
				height += InterpolatedNoise(x / freq, z / freq) * Amplitude * rough;
			}

			return height;
		}

		private float InterpolatedNoise(float x, float z)
		{
			var xi = (int)x;
			var zi = (int)z;

			var xf = x - xi;
			var zf = z - zi;

			var v1 = SmoothNoise(xi, zi);
			var v2 = SmoothNoise(xi + 1, zi);
			var v3 = SmoothNoise(xi, zi + 1);
			var v4 = SmoothNoise(xi + 1, zi + 1);

			var i1 = algebra.cerp(v1, v2, xf);
			var i2 = algebra.cerp(v3, v4, xf);

			var i = algebra.cerp(i1, i2, zf);

			return i;
		}
		private float SmoothNoise(int x, int z)
		{
			var corners =
			(
				CoarseNoise(x - 1, z - 1)
				+
				CoarseNoise(x + 1, z - 1)
				+
				CoarseNoise(x - 1, z + 1)
				+
				CoarseNoise(x + 1, z + 1)
			) / 16.0f;

			var sides =
			(
				CoarseNoise(x - 1, z)
				+
				CoarseNoise(x + 1, z)
				+
				CoarseNoise(x, z - 1)
				+
				CoarseNoise(x, z + 1)
			) / 8.0f;

			var center = CoarseNoise(x, z) / 4.0f;

			return corners + sides + center;
		}
		private float CoarseNoise(int x, int z)
		{
			if (x < 0 || Granularity < x) return 0.0f;
			if (z < 0 || Granularity < z) return 0.0f;
			return Noise[x, z];
		}
	}
}

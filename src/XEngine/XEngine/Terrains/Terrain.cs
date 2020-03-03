using System;
using System.Drawing;

using GlmNet;

namespace XEngine.Terrains
{
	using XEngine.Shading;
	using XEngine.Common;

	public class Terrain
	{
		public static Terrain GenerateFlat(float length, uint tiles, uint granularity = 1u)
		{
			var terrain = new Terrain(length, tiles, granularity);
			terrain.Generate();
			terrain.Shape = new GeometricShape(new ShapeData(terrain.Vertices, terrain.Indices));
			return terrain;
		}
		public static Terrain Generate(float length, uint tiles, HeightMap heightmap)
		{
			if (heightmap == null) throw new ArgumentNullException(nameof(heightmap));
			var terrain = new Terrain(length, tiles, heightmap.Granularity);
			terrain.Generate(heightmap);
			terrain.Shape = new GeometricShape(new ShapeData(terrain.Vertices, terrain.Indices));
			return terrain;
		}

		public float Length { get; }
		public uint Tiles { get; }
		public uint Granularity { get; }

		private vertex[] Vertices = null;
		private int[] Indices = null;

		public float MaxHeight { get; private set; } = float.NegativeInfinity;
		public float MinHeight { get; private set; } = float.PositiveInfinity;
		
		public GeometricShape Shape { get; private set; }

		private Terrain(float length, uint tiles, uint granularity)
		{
			if (length <= 0.0f) throw new ArgumentException("Length must be positive.");
			if (granularity == 0) throw new ArgumentException("Granularity must be positive.");
			if (tiles == 0) throw new ArgumentException("Tiles must be positive.");
			
			Length = length;
			Granularity = granularity;
			Tiles = tiles;
		}

		private void Generate(HeightMap heightmap = null)
		{
			var vert_count = Granularity + 1u;
			int index(uint x, uint z) => (int)(x + z * vert_count);

			Vertices = new vertex[vert_count * vert_count];
			Indices = new int[Granularity * Granularity * 6];

			var color = new vec3(+0.0f, +0.0f, +0.0f);
			var delta_xz = Length / Granularity;
			var delta_uv = (float)Tiles / Granularity;

			for (var z = 0u; z < vert_count; ++z)
			{
				for (var x = 0u; x < vert_count; ++x)
				{
					var y = heightmap?.GetHeight(x, z) ?? 0.0f;

					if (y > MaxHeight) MaxHeight = y;
					if (y < MinHeight) MinHeight = y;
					
					Vertices[index(x, z)] = new vertex
					(
						new vec3(x * delta_xz - Length / 2.0f, y, z * delta_xz - Length / 2.0f),
						color,
						vector3.up,
						new vec2(x * delta_uv, z * delta_uv)
					);
				}
			}

			for (var z = 0u; z < Granularity; ++z)
			{
				for (var x = 0u; x < Granularity; ++x)
				{
					var i = z * Granularity + x;
					Indices[i * 6u + 0u] = index(x, z);
					Indices[i * 6u + 1u] = index(x, z + 1u);
					Indices[i * 6u + 2u] = index(x + 1u, z);
					Indices[i * 6u + 3u] = index(x + 1u, z);
					Indices[i * 6u + 4u] = index(x, z + 1u);
					Indices[i * 6u + 5u] = index(x + 1u, z + 1u);
				}
			}

			if (heightmap == null) return;

			for (var z = 0u; z < vert_count; ++z)
			{
				for (var x = 0u; x < vert_count; ++x)
				{
					var li = (int)x - 1;
					var ri = (int)x + 1;
					var di = (int)z - 1;
					var ui = (int)z + 1;

					var l = (li < 0) ? 0.0f : Vertices[index((uint)li, z)].position.y;
					var r = (ri >= vert_count) ? 0.0f : Vertices[index((uint)ri, z)].position.y;
					var d = (di < 0) ? 0.0f : Vertices[index(x, (uint)di)].position.y;
					var u = (ui >= vert_count) ? 0.0f : Vertices[index(x, (uint)ui)].position.y;

					Vertices[index(x, z)].normal = new vec3(l - r, 2.0f, d - u).normalize();
				}
			}
		}

		public float CalculateLocalHeight(vec2 point) => CalculateLocalHeight(point.x, point.y);
		public float CalculateLocalHeight(float x, float z)
		{
			var vert_count = (int)Granularity + 1;
			var unit_width = Length / Granularity;
			int index(int x_i, int z_i) => x_i + z_i * vert_count;

			var l2 = +Length / 2.0f;

			if (x < -l2 || +l2 < x) throw new ArgumentException("Invalid x position.");
			if (z < -l2 || +l2 < z) throw new ArgumentException("Invalid z position.");

			var xn = x + l2;
			var zn = z + l2;

			var xi = (int)(xn / unit_width);
			var zi = (int)(zn / unit_width);

			if (xi == Granularity) --xi;
			if (zi == Granularity) --zi;

			var xo = xn % unit_width;
			var zo = zn % unit_width;

			var p0 = Vertices[index(xi + 0, zi + 0)].position;
			var p1 = Vertices[index(xi + 1, zi + 0)].position;
			var p2 = Vertices[index(xi + 0, zi + 1)].position;
			var p3 = Vertices[index(xi + 1, zi + 1)].position;

			var point = new vec2(x, z);

			return xo + zo >= 1.0f ? point.bcerp(p1, p2, p3) : point.bcerp(p0, p2, p1);
		}

		public Bitmap ToBitmap(uint granularity)
		{
			var vert_count = granularity + 1u;
			var d = Length / granularity;
			var l2 = Length / 2.0f;
			
			var bmp = new Bitmap((int)vert_count, (int)vert_count);

			for (var z = 0u; z < vert_count; ++z)
			{
				for (var x = 0u; x < vert_count; ++x)
				{
					var xf = x * d - l2;
					var zf = z * d - l2;
					var y = CalculateLocalHeight(xf, zf);
					var b = (int)(255 * (y - MinHeight) / (MaxHeight - MinHeight));
					bmp.SetPixel((int)x, (int)z, System.Drawing.Color.FromArgb(255, b, b, b));
				}
			}

			return bmp;
		}
	}
}

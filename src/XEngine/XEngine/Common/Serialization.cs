using System;
using GlmNet;

namespace XEngine.Common
{
	using XEngine.Shading;

	public static class Serialization
	{
		public static float[] serialize(this Color[] colors, float[] array = null, bool rgb_only = false)
		{
			if (colors == null) throw new ArgumentNullException(nameof(colors));

			if (rgb_only)
			{
				array = array ?? new float[colors.Length * 3];
				if (array.Length != colors.Length * 3) throw new ArgumentException("Invalid array length.");

				for (var i = 0; i < colors.Length; ++i)
				{
					array[i * 3 + 0] = colors[i].r;
					array[i * 3 + 1] = colors[i].g;
					array[i * 3 + 2] = colors[i].b;
				}

				return array;
			}
			else
			{
				array = array ?? new float[colors.Length * 4];
				if (array.Length != colors.Length * 4) throw new ArgumentException("Invalid array length.");

				for (var i = 0; i < colors.Length; ++i)
				{
					array[i * 4 + 0] = colors[i].r;
					array[i * 4 + 1] = colors[i].g;
					array[i * 4 + 2] = colors[i].b;
					array[i * 4 + 3] = colors[i].a;
				}

				return array;
			}
		}
		public static float[] serialize(this vec2[] values, float[] array = null)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));
			array = array ?? new float[values.Length * 2];
			if (array.Length != values.Length * 2) throw new ArgumentException("Invalid array length.");

			for (var i = 0; i < values.Length; ++i)
			{
				array[i * 2 + 0] = values[i].x;
				array[i * 2 + 1] = values[i].y;
			}

			return array;
		}
		public static float[] serialize(this vec3[] values, float[] array = null)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));
			array = array ?? new float[values.Length * 3];
			if (array.Length != values.Length * 3) throw new ArgumentException("Invalid array length.");

			for (var i = 0; i < values.Length; ++i)
			{
				array[i * 3 + 0] = values[i].x;
				array[i * 3 + 1] = values[i].y;
				array[i * 3 + 2] = values[i].z;
			}

			return array;
		}
		public static float[] serialize(this vec4[] values, float[] array = null)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));
			array = array ?? new float[values.Length * 4];
			if (array.Length != values.Length * 4) throw new ArgumentException("Invalid array length.");

			for (var i = 0; i < values.Length; ++i)
			{
				array[i * 4 + 0] = values[i].x;
				array[i * 4 + 1] = values[i].y;
				array[i * 4 + 2] = values[i].z;
				array[i * 4 + 3] = values[i].w;
			}

			return array;
		}
		public static float[] serialize(this mat2 value, float[] array = null)
		{
			array = array ?? new float[4];
			if (array.Length != 4) throw new ArgumentException("Invalid array length.");
			for (var c = 0; c < 4; ++c) array[c] = value[c / 2, c % 2];
			return array;
		}
		public static float[] serialize(this mat3 value, float[] array = null)
		{
			array = array ?? new float[9];
			if (array.Length != 9) throw new ArgumentException("Invalid array length.");
			for (var c = 0; c < 9; ++c) array[c] = value[c / 3, c % 3];
			return array;
		}
		public static float[] serialize(this mat4 value, float[] array = null)
		{
			array = array ?? new float[16];
			if (array.Length != 16) throw new ArgumentException("Invalid array length.");
			for (var c = 0; c < 16; ++c) array[c] = value[c / 4, c % 4];
			return array;
		}
		public static float[] serialize(this mat2[] values, float[] array = null)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));
			array = array ?? new float[values.Length * 4];
			if (array.Length != values.Length * 4) throw new ArgumentException("Invalid array length.");

			for (var i = 0; i < values.Length; ++i)
				for (var c = 0; c < 4; ++c) 
					array[i * 4 + c] = values[i][c / 2, c % 2];

			return array;
		}
		public static float[] serialize(this mat3[] values, float[] array = null)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));
			array = array ?? new float[values.Length * 9];
			if (array.Length != values.Length * 9) throw new ArgumentException("Invalid array length.");

			for (var i = 0; i < values.Length; ++i)
				for (var c = 0; c < 9; ++c)
					array[i * 9 + c] = values[i][c / 3, c % 3];

			return array;
		}
		public static float[] serialize(this mat4[] values, float[] array = null)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));
			array = array ?? new float[values.Length * 16];
			if (array.Length != values.Length * 16) throw new ArgumentException("Invalid array length.");

			for (var i = 0; i < values.Length; ++i)
				for (var c = 0; c < 16; ++c)
					array[i * 16 + c] = values[i][c / 4, c % 4];

			return array;
		}
	}
}

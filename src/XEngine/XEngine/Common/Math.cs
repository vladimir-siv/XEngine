using System;
using System.Drawing;
using GlmNet;

namespace XEngine.Common
{
	using XEngine.Shading;

	public static class scalar
	{
		public static float Clamp(this float v, float min, float max)
		{
			if (v < min) return min;
			if (v > max) return max;
			return v;
		}
		public static uint BitCount(this uint v)
		{
			v -= ((v >> 1) & 0x55555555);
			v = (v & 0x33333333) + ((v >> 2) & 0x33333333);
			return ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
		}
		public static float ToRad(this float v)
		{
			return v * (float)Math.PI / 180.0f;
		}
		public static float ToDeg(this float v)
		{
			return v * 180.0f / (float)Math.PI;
		}
		public static vec2 ToOffsets(this uint v, uint rank)
		{
			return new vec2((v % rank) / (float)rank, (v / rank) / (float)rank);
		}
	}

	public static class vector2
	{
		public static readonly vec2 zero		= new vec2(+0.0f, +0.0f);
		public static readonly vec2 one			= new vec2(+1.0f, +1.0f);

		public static float bcerp(this vec2 pos, vec3 p1, vec3 p2, vec3 p3)
		{
			float det = (p2.z - p3.z) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.z - p3.z);
			float l1 = ((p2.z - p3.z) * (pos.x - p3.x) + (p3.x - p2.x) * (pos.y - p3.z)) / det;
			float l2 = ((p3.z - p1.z) * (pos.x - p3.x) + (p1.x - p3.x) * (pos.y - p3.z)) / det;
			float l3 = 1.0f - l1 - l2;
			return l1 * p1.y + l2 * p2.y + l3 * p3.y;
		}

		public static vec2 to_vec2(this Point point) => new vec2(point.X, point.Y);
	}

	public static class vector3
	{
		public static readonly vec3 zero		= new vec3(+0.0f, +0.0f, +0.0f);
		public static readonly vec3 one			= new vec3(+1.0f, +1.0f, +1.0f);
		public static readonly vec3 forward		= new vec3(+0.0f, +0.0f, -1.0f);
		public static readonly vec3 backward	= new vec3(+0.0f, +0.0f, +1.0f);
		public static readonly vec3 right		= new vec3(+1.0f, +0.0f, +0.0f);
		public static readonly vec3 left		= new vec3(-1.0f, +0.0f, +0.0f);
		public static readonly vec3 up			= new vec3(+0.0f, +1.0f, +0.0f);
		public static readonly vec3 down		= new vec3(+0.0f, -1.0f, +0.0f);

		public static vec3 normalize(this vec3 v) => v != zero ? glm.normalize(v) : zero;
		public static vec2 to_vec2(this vec3 v) => new vec2(v.x, v.y);
		public static vec4 to_vec4(this vec3 v) => new vec4(v.x, v.y, v.z, 1.0f);
		public static float distance(this vec3 v1, vec3 v2)
		{
			var xd = v2.x - v1.x;
			var yd = v2.y - v1.y;
			var zd = v2.z - v1.z;
			return (float)Math.Sqrt(xd * xd + yd * yd + zd * zd);
		}
	}

	public static class vector4
	{
		public static readonly vec4 neutral		= new vec4(+0.0f, +0.0f, +0.0f, +1.0f);

		public static readonly vec4 zero		= new vec4(+0.0f, +0.0f, +0.0f, +0.0f);
		public static readonly vec4 one			= new vec4(+1.0f, +1.0f, +1.0f, +1.0f);
		public static readonly vec4 forward		= new vec4(+0.0f, +0.0f, -1.0f, +1.0f);
		public static readonly vec4 backward	= new vec4(+0.0f, +0.0f, +1.0f, +1.0f);
		public static readonly vec4 right		= new vec4(+1.0f, +0.0f, +0.0f, +1.0f);
		public static readonly vec4 left		= new vec4(-1.0f, +0.0f, +0.0f, +1.0f);
		public static readonly vec4 up			= new vec4(+0.0f, +1.0f, +0.0f, +1.0f);
		public static readonly vec4 down		= new vec4(+0.0f, -1.0f, +0.0f, +1.0f);

		public static vec4 normalize(this vec4 v) => v != zero ? glm.normalize(v) : zero;
		public static vec3 to_vec3(this vec4 v, bool suppress = false) => suppress ? new vec3(v.x / v.w, v.y / v.w, v.z / v.w) : new vec3(v.x, v.y, v.z);
		public static float distance(this vec4 v1, vec4 v2)
		{
			var xd = v2.x - v1.x;
			var yd = v2.y - v1.y;
			var zd = v2.z - v1.z;
			var wd = v2.w - v1.w;
			return (float)Math.Sqrt(xd * xd + yd * yd + zd * zd + wd * wd);
		}
	}

	public static class quaternion
	{
		public static vec3 calculate_position(this mat4 mat) => (mat * vector4.neutral).to_vec3();

		public static mat4 euler(vec3 xyz) => euler(mat4.identity(), xyz.x, xyz.y, xyz.z);
		public static mat4 euler(float x, float y, float z) => euler(mat4.identity(), x, y, z);
		public static mat4 euler(mat4 mat, vec3 xyz) => euler(mat, xyz.x, xyz.y, xyz.z);
		public static mat4 euler(mat4 mat, float x, float y, float z)
		{
			return glm.rotate(glm.rotate(glm.rotate(mat, y.ToRad(), vector3.up), x.ToRad(), vector3.right), z.ToRad(), vector3.backward);
		}

		public static mat4 identify(this mat4 mat)
		{
			mat[0] = new vec4(1.0f, 0.0f, 0.0f, 0.0f);
			mat[1] = new vec4(0.0f, 1.0f, 0.0f, 0.0f);
			mat[2] = new vec4(0.0f, 0.0f, 1.0f, 0.0f);
			mat[3] = new vec4(0.0f, 0.0f, 0.0f, 1.0f);
			return mat;
		}
		public static mat4 clone(this mat4 mat)
		{
			var clone = mat4.identity();
			clone[0] = mat[0];
			clone[1] = mat[1];
			clone[2] = mat[2];
			clone[3] = mat[3];
			return clone;
		}
		public static mat4 copy_to(this mat4 src, mat4 dest)
		{
			dest[0] = src[0];
			dest[1] = src[1];
			dest[2] = src[2];
			dest[3] = src[3];
			return dest;
		}
	}

	public static class algebra
	{
		public static float lerp(float v1, float v2, float a) => v1 * (1.0f - a) + v2 * a;
		public static vec2 lerp(vec2 v1, vec2 v2, float a) => v1 * (1.0f - a) + v2 * a;
		public static vec3 lerp(vec3 v1, vec3 v2, float a) => v1 * (1.0f - a) + v2 * a;
		public static vec4 lerp(vec4 v1, vec4 v2, float a) => v1 * (1.0f - a) + v2 * a;
		public static Color lerp(Color v1, Color v2, float a) => v1 * (1.0f - a) + v2 * a;

		public static float cerp(float a, float b, float blend)
		{
			var theta = blend * Math.PI;
			var f = (float)(1.0f - Math.Cos(theta)) * 0.5f;
			return a * (1.0f - f) + b * f;
		}
	}
}

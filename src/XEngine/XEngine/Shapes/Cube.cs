using GlmNet;

namespace XEngine.Shapes
{
	using XEngine.Shading;

	public class Cube : GeometricShape
	{
		public Cube() :
			this
			(
				-0.5f, +0.5f, +0.5f,
				+0.5f, +0.5f, +0.5f,
				+0.5f, +0.5f, -0.5f,
				-0.5f, +0.5f, -0.5f,
				-0.5f, -0.5f, +0.5f,
				+0.5f, -0.5f, +0.5f,
				+0.5f, -0.5f, -0.5f,
				-0.5f, -0.5f, -0.5f,

				1, 0, 0,
				0, 1, 0,
				0, 0, 1,

				1, 1, 1,
				1, 0, 0,
				0, 1, 0,

				0, 0, 1,
				1, 1, 1,
				1, 0, 0,

				0, 1, 0,
				0, 0, 1,
				1, 1, 1,

				1, 1, 1,
				0, 0, 1,
				0, 1, 0,

				1, 0, 0,
				1, 1, 1,
				0, 0, 1,

				0, 1, 0,
				1, 0, 0,
				1, 1, 1,

				0, 0, 1,
				0, 1, 0,
				1, 0, 0

				/*1, 0, 0,
				0, 1, 0,
				0, 0, 1,

				1, 0, 0,
				0, 0, 1,
				1, 1, 0,

				1, 0, 0,
				1, 1, 0,
				0, 1, 1,

				1, 0, 0,
				0, 1, 1,
				0, 1, 0,

				1, 0, 1,
				0, 1, 0,
				0, 0, 1,

				1, 0, 1,
				0, 0, 1,
				1, 1, 0,

				1, 0, 1,
				1, 1, 0,
				0, 1, 1,

				1, 0, 1,
				0, 1, 1,
				0, 1, 0*/
			)
		{

		}

		public Cube
		(
			float x1, float y1, float z1,
			float x2, float y2, float z2,
			float x3, float y3, float z3,
			float x4, float y4, float z4,
			float x5, float y5, float z5,
			float x6, float y6, float z6,
			float x7, float y7, float z7,
			float x8, float y8, float z8,

			float c1dr, float c1dg, float c1db,
			float c1lr, float c1lg, float c1lb,
			float c1rr, float c1rg, float c1rb,

			float c2dr, float c2dg, float c2db,
			float c2lr, float c2lg, float c2lb,
			float c2rr, float c2rg, float c2rb,

			float c3dr, float c3dg, float c3db,
			float c3lr, float c3lg, float c3lb,
			float c3rr, float c3rg, float c3rb,

			float c4dr, float c4dg, float c4db,
			float c4lr, float c4lg, float c4lb,
			float c4rr, float c4rg, float c4rb,

			float c5dr, float c5dg, float c5db,
			float c5lr, float c5lg, float c5lb,
			float c5rr, float c5rg, float c5rb,

			float c6dr, float c6dg, float c6db,
			float c6lr, float c6lg, float c6lb,
			float c6rr, float c6rg, float c6rb,

			float c7dr, float c7dg, float c7db,
			float c7lr, float c7lg, float c7lb,
			float c7rr, float c7rg, float c7rb,

			float c8dr, float c8dg, float c8db,
			float c8lr, float c8lg, float c8lb,
			float c8rr, float c8rg, float c8rb
		) :
			this
			(
				new vec3(x1, y1, z1),
				new vec3(x2, y2, z2),
				new vec3(x3, y3, z3),
				new vec3(x4, y4, z4),
				new vec3(x5, y5, z5),
				new vec3(x6, y6, z6),
				new vec3(x7, y7, z7),
				new vec3(x8, y8, z8),

				new vec3(c1dr, c1dg, c1db),
				new vec3(c1lr, c1lg, c1lb),
				new vec3(c1rr, c1rg, c1rb),

				new vec3(c2dr, c2dg, c2db),
				new vec3(c2lr, c2lg, c2lb),
				new vec3(c2rr, c2rg, c2rb),

				new vec3(c3dr, c3dg, c3db),
				new vec3(c3lr, c3lg, c3lb),
				new vec3(c3rr, c3rg, c3rb),

				new vec3(c4dr, c4dg, c4db),
				new vec3(c4lr, c4lg, c4lb),
				new vec3(c4rr, c4rg, c4rb),

				new vec3(c5dr, c5dg, c5db),
				new vec3(c5lr, c5lg, c5lb),
				new vec3(c5rr, c5rg, c5rb),

				new vec3(c6dr, c6dg, c6db),
				new vec3(c6lr, c6lg, c6lb),
				new vec3(c6rr, c6rg, c6rb),

				new vec3(c7dr, c7dg, c7db),
				new vec3(c7lr, c7lg, c7lb),
				new vec3(c7rr, c7rg, c7rb),

				new vec3(c8dr, c8dg, c8db),
				new vec3(c8lr, c8lg, c8lb),
				new vec3(c8rr, c8rg, c8rb)
			)
		{

		}

		public Cube
		(
			vec3 p1, vec3 p2, vec3 p3, vec3 p4, vec3 p5, vec3 p6, vec3 p7, vec3 p8,
			vec3 c1d, vec3 c1l, vec3 c1r,
			vec3 c2d, vec3 c2l, vec3 c2r,
			vec3 c3d, vec3 c3l, vec3 c3r,
			vec3 c4d, vec3 c4l, vec3 c4r,
			vec3 c5d, vec3 c5l, vec3 c5r,
			vec3 c6d, vec3 c6l, vec3 c6r,
			vec3 c7d, vec3 c7l, vec3 c7r,
			vec3 c8d, vec3 c8l, vec3 c8r
		) :
			this
			(
				new vertex[]
				{
					// Top side
					new vertex(p1, c1d, new vec3(+0.0f, +1.0f, +0.0f), new vec2(0.0f, 1.0f)),		// [0]
					new vertex(p2, c2d, new vec3(+0.0f, +1.0f, +0.0f), new vec2(1.0f, 1.0f)),		// [1]
					new vertex(p4, c4d, new vec3(+0.0f, +1.0f, +0.0f), new vec2(0.0f, 0.0f)),		// [2]

					new vertex(p3, c3d, new vec3(+0.0f, +1.0f, +0.0f), new vec2(1.0f, 0.0f)),		// [3]
					//new vertex(p4, c4d, new vec3(+0.0f, +1.0f, +0.0f), new vec2(0.0f, 0.0f)),		// <2>
					//new vertex(p2, c2d, new vec3(+0.0f, +1.0f, +0.0f), new vec2(1.0f, 1.0f)),		// <1>
					
					// Bottom side
					new vertex(p5, c5d, new vec3(+0.0f, -1.0f, +0.0f), new vec2(0.0f, 0.0f)),		// [4]
					new vertex(p8, c8d, new vec3(+0.0f, -1.0f, +0.0f), new vec2(0.0f, 1.0f)),		// [5]
					new vertex(p6, c6d, new vec3(+0.0f, -1.0f, +0.0f), new vec2(1.0f, 0.0f)),		// [6]

					new vertex(p7, c7d, new vec3(+0.0f, -1.0f, +0.0f), new vec2(1.0f, 1.0f)),		// [7]
					//new vertex(p6, c6d, new vec3(+0.0f, -1.0f, +0.0f), new vec2(1.0f, 0.0f)),		// <6>
					//new vertex(p8, c8d, new vec3(+0.0f, -1.0f, +0.0f), new vec2(0.0f, 1.0f)),		// <5>

					// Front side
					new vertex(p5, c5r, new vec3(+0.0f, +0.0f, +1.0f), new vec2(0.0f, 1.0f)),		// [8]
					new vertex(p6, c6l, new vec3(+0.0f, +0.0f, +1.0f), new vec2(1.0f, 1.0f)),		// [9]
					new vertex(p1, c1r, new vec3(+0.0f, +0.0f, +1.0f), new vec2(0.0f, 0.0f)),		// [10]

					new vertex(p2, c2l, new vec3(+0.0f, +0.0f, +1.0f), new vec2(1.0f, 0.0f)),		// [11]
					//new vertex(p1, c1r, new vec3(+0.0f, +0.0f, +1.0f), new vec2(0.0f, 0.0f)),		// <10>
					//new vertex(p6, c6l, new vec3(+0.0f, +0.0f, +1.0f), new vec2(1.0f, 1.0f)),		// <9>

					// Right side
					new vertex(p6, c6r, new vec3(+1.0f, +0.0f, +0.0f), new vec2(0.0f, 1.0f)),		// [12]
					new vertex(p7, c7l, new vec3(+1.0f, +0.0f, +0.0f), new vec2(1.0f, 1.0f)),		// [13]
					new vertex(p2, c2r, new vec3(+1.0f, +0.0f, +0.0f), new vec2(0.0f, 0.0f)),		// [14]

					new vertex(p3, c3l, new vec3(+1.0f, +0.0f, +0.0f), new vec2(1.0f, 0.0f)),		// [15]
					//new vertex(p2, c2r, new vec3(+1.0f, +0.0f, +0.0f), new vec2(0.0f, 0.0f)),		// <14>
					//new vertex(p7, c7l, new vec3(+1.0f, +0.0f, +0.0f), new vec2(1.0f, 1.0f)),		// <13>

					// Back side
					new vertex(p7, c7r, new vec3(+0.0f, +0.0f, -1.0f), new vec2(0.0f, 1.0f)),		// [16]
					new vertex(p8, c8l, new vec3(+0.0f, +0.0f, -1.0f), new vec2(1.0f, 1.0f)),		// [17]
					new vertex(p3, c3r, new vec3(+0.0f, +0.0f, -1.0f), new vec2(0.0f, 0.0f)),		// [18]

					new vertex(p4, c4l, new vec3(+0.0f, +0.0f, -1.0f), new vec2(1.0f, 0.0f)),		// [19]
					//new vertex(p3, c3r, new vec3(+0.0f, +0.0f, -1.0f), new vec2(0.0f, 0.0f)),		// <18>
					//new vertex(p8, c8l, new vec3(+0.0f, +0.0f, -1.0f), new vec2(1.0f, 1.0f)),		// <17>

					// Left side
					new vertex(p8, c8r, new vec3(-1.0f, +0.0f, +0.0f), new vec2(0.0f, 1.0f)),		// [20]
					new vertex(p5, c5l, new vec3(-1.0f, +0.0f, +0.0f), new vec2(1.0f, 1.0f)),		// [21]
					new vertex(p4, c4r, new vec3(-1.0f, +0.0f, +0.0f), new vec2(0.0f, 0.0f)),		// [22]

					new vertex(p1, c1l, new vec3(-1.0f, +0.0f, +0.0f), new vec2(1.0f, 0.0f)),		// [23]
					//new vertex(p4, c4r, new vec3(-1.0f, +0.0f, +0.0f), new vec2(0.0f, 0.0f)),		// <22>
					//new vertex(p5, c5l, new vec3(-1.0f, +0.0f, +0.0f), new vec2(1.0f, 1.0f)),		// <21>
				},
				new int[]
				{
					 0,  1,  2,  3,    2,  1,
					 4,  5,  6,  7,    6,  5,
					 8,  9, 10, 11,   10,  9,
					12, 13, 14, 15,   14, 13,
					16, 17, 18, 19,   18, 17,
					20, 21, 22, 23,   22, 21,
				}
			)
		{

		}

		private Cube(vertex[] vertices, int[] indices = null) : base(new ShapeData(vertices, indices))
		{

		}
	}
}

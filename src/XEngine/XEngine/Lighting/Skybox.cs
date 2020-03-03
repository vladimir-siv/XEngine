namespace XEngine.Lighting
{
	using XEngine.Core;
	using XEngine.Shading;

	public struct Skybox
	{
		internal class SkyboxShape : GeometricShape
		{
			public static GeometricShape Geometry { get; } = new SkyboxShape();

			private SkyboxShape() : base
			(
				new ShapeData
				(
					ShapeData.Positions3f
					(
						-0.5f, +0.5f, -0.5f,
						-0.5f, -0.5f, -0.5f,
						+0.5f, -0.5f, -0.5f,
						+0.5f, -0.5f, -0.5f,
						+0.5f, +0.5f, -0.5f,
						-0.5f, +0.5f, -0.5f,

						-0.5f, -0.5f, +0.5f,
						-0.5f, -0.5f, -0.5f,
						-0.5f, +0.5f, -0.5f,
						-0.5f, +0.5f, -0.5f,
						-0.5f, +0.5f, +0.5f,
						-0.5f, -0.5f, +0.5f,

						+0.5f, -0.5f, -0.5f,
						+0.5f, -0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, -0.5f,
						+0.5f, -0.5f, -0.5f,

						-0.5f, -0.5f, +0.5f,
						-0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, -0.5f, +0.5f,
						-0.5f, -0.5f, +0.5f,

						-0.5f, +0.5f, -0.5f,
						+0.5f, +0.5f, -0.5f,
						+0.5f, +0.5f, +0.5f,
						+0.5f, +0.5f, +0.5f,
						-0.5f, +0.5f, +0.5f,
						-0.5f, +0.5f, -0.5f,

						-0.5f, -0.5f, -0.5f,
						-0.5f, -0.5f, +0.5f,
						+0.5f, -0.5f, -0.5f,
						+0.5f, -0.5f, -0.5f,
						-0.5f, -0.5f, +0.5f,
						+0.5f, -0.5f, +0.5f
					)
				)
			)
			{
				Attributes = VertexAttribute.POSITION;
				KeepAlive = true;
			}
		}

		private static Mesh _mesh = null;
		internal static Mesh mesh
		{
			get
			{
				return _mesh;
			}
			set
			{
				if (value == _mesh) return;
				_mesh?.Dispose();
				_mesh = value;
			}
		}

		public static Skybox Black { get; } = new Skybox("$BuiltIn/Black", Color.Black);
		public static Skybox Default { get; } = new Skybox("$BuiltIn/Default", Color.DeepSky);

		public static Skybox Find(string name) => Find(name, Color.Gray);
		public static Skybox Find(string name, Color skycolor)
		{
			var skybox = new Skybox(name, skycolor);
			skybox.texture = CubeMap.Find($"Skyboxes/{name}");
			mesh = mesh ?? new Mesh() { shape = SkyboxShape.Geometry };
			return skybox;
		}

		public string Id { get; }
		public Color SkyColor { get; }
		internal Texture texture;

		private Skybox(string id, Color skycolor)
		{
			Id = id;
			SkyColor = skycolor;
			texture = null;
		}
	}
}

using System;

using GlmNet;

namespace XEngine.Core
{
	using XEngine.Shading;
	using XEngine.Common;

	public class Prefab
	{
		public string name;

		public Mesh mesh;
		public Material material;

		public Prefab(string name)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentException("Prefab name cannot be null or empty.");
			this.name = name;
		}

		public GameObject Instantiate() => Instantiate(Transform.Origin);
		public GameObject Instantiate(vec3 position) => Instantiate(new Transform(position, vector3.zero, vector3.one));
		public GameObject Instantiate(Transform transform)
		{
			var instance = new GameObject(name);
			instance.mesh = mesh;
			instance.material = material;
			instance.transform = transform;
			return instance;
		}
	}
}

using GlmNet;

namespace XEngine.Core
{
	using XEngine.Common;

	public struct Transform
	{
		public static readonly Transform Origin = new Transform(vector3.zero, vector3.zero, vector3.one);

		public vec3 position;
		public vec3 rotation;
		public vec3 scale;

		public Transform(vec3 position, vec3 rotation, vec3 scale)
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}

		public SpaceUnits WorldSpaceUnits
		{
			get
			{
				var transform4d = quaternion.euler(rotation);

				return new SpaceUnits
				(
					(transform4d * vector4.forward).to_vec3(),
					(transform4d * vector4.right).to_vec3(),
					(transform4d * vector4.up).to_vec3()
				);
			}
		}
	}

	public struct SpaceUnits
	{
		public vec3 forward;
		public vec3 right;
		public vec3 up;

		public SpaceUnits(vec3 forward, vec3 right, vec3 up)
		{
			this.forward = forward;
			this.right = right;
			this.up = up;
		}
	}
}

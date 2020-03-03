using System.Collections.Generic;

namespace XEngine.Shading
{
	public class ShaderProperties
	{
		private Dictionary<string, uint> AtlasIndices = new Dictionary<string, uint>();

		public bool TryGetAtlasIndex(string texture, out uint index) => AtlasIndices.TryGetValue(texture, out index);
		public uint GetAtlasIndex(string texture) => AtlasIndices[texture];
		public void SetAtlasIndex(string texture, uint index) => AtlasIndices[texture] = index;
		public bool RemoveAtlasIndex(string texture) => AtlasIndices.Remove(texture);
	}
}

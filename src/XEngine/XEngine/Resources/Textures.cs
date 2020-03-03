using System.Drawing;

namespace XEngine.Resources
{
	public static partial class Resource
	{
		public static Bitmap LoadTexture(string textureName)
		{
			using (var stream = ManifestResourceManager.LoadFromResources($"Textures/{textureName}.bmp"))
			{
				return new Bitmap(stream);
			}
		}
		public static Bitmap LoadCustomTexture(string textureName)
		{
			using (var stream = ManifestResourceManager.LoadFromResources($"Textures/{textureName}"))
			{
				using (var image = Image.FromStream(stream))
				{
					return new Bitmap(image);
				}
			}
		}
	}
}

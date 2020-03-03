using System;
using System.Drawing;

namespace XEngine.Terrains
{
	using XEngine.Resources;

	public class TextureHeightMap : HeightMap, IDisposable
	{
		private Bitmap Texture = null;
		public float Amplitude { get; set; } = 50.0f;
		public bool Negative { get; set; } = true;

		public TextureHeightMap(string textureName)
		{
			Texture = Resource.LoadCustomTexture(textureName);

			if (Texture.Width != Texture.Height)
			{
				Texture.Dispose();
				throw new FormatException("The texture must be of the same width and height.");
			}

			Granularity = (uint)Texture.Width - 1u;
		}

		public override float GetHeight(float x, float z)
		{
			var h = Texture.GetPixel((int)x, (int)z).GetBrightness();
			if (Negative) h = (h - 0.5f) * 2.0f;
			return h * Amplitude;
		}

		public void Dispose()
		{
			Texture.Dispose();
			Texture = null;
		}
	}
}

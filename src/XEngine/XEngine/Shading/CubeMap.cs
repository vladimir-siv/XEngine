using System;
using System.Drawing;
using System.Drawing.Imaging;

using SharpGL;

namespace XEngine.Shading
{
	using XEngine.Resources;

	public class CubeMap : Texture
	{
		private static string Side(uint i)
		{
			switch (i)
			{
				case 0u: return "right";
				case 1u: return "left";
				case 2u: return "top";
				case 3u: return "bottom";
				case 4u: return "back";
				case 5u: return "front";
				default: return null;
			}
		}

		public static Texture Find(string name)
		{
			var name_key = name.ToLower();

			if (XEngineContext.Textures.TryGetValue(name_key, out var found))
			{
				return found;
			}

			var side0 = Resource.LoadCustomTexture($"{name}/{Side(0)}.png");
			var side1 = Resource.LoadCustomTexture($"{name}/{Side(1)}.png");
			var side2 = Resource.LoadCustomTexture($"{name}/{Side(2)}.png");
			var side3 = Resource.LoadCustomTexture($"{name}/{Side(3)}.png");
			var side4 = Resource.LoadCustomTexture($"{name}/{Side(4)}.png");
			var side5 = Resource.LoadCustomTexture($"{name}/{Side(5)}.png");

			var texture = new CubeMap();
			texture.Load(side0, side1, side2, side3, side4, side5);

			side0.Dispose();
			side1.Dispose();
			side2.Dispose();
			side3.Dispose();
			side4.Dispose();
			side5.Dispose();

			XEngineContext.Textures.Add(name_key, texture);
			return texture;
		}

		protected override uint TextureType => OpenGL.GL_TEXTURE_CUBE_MAP;

		private CubeMap() { }

		private void Load(params Bitmap[] sides)
		{
			if (sides == null) throw new ArgumentNullException(nameof(sides));
			if (sides.Length != 6) throw new InvalidOperationException("Cubemap must take 6 sides.");

			var gl = XEngineContext.Graphics;

			Activate();
			//gl.BindTexture(OpenGL.GL_TEXTURE_CUBE_MAP, TextureId);

			for (var i = 0u; i < 6u; ++i)
			{
				var bitmap = sides[i];
				var bitmapData = sides[i].LockBits
				(
					new Rectangle(0, 0, bitmap.Width, bitmap.Height),
					ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb
				);

				gl.TexImage2D
				(
					OpenGL.GL_TEXTURE_CUBE_MAP_POSITIVE_X + i,
					0,
					OpenGL.GL_RGBA,
					bitmap.Width,
					bitmap.Height,
					0,
					OpenGL.GL_BGRA,
					OpenGL.GL_UNSIGNED_BYTE,
					bitmapData.Scan0
				);

				bitmap.UnlockBits(bitmapData);
			}

			gl.GenerateMipmapEXT(OpenGL.GL_TEXTURE_CUBE_MAP);
			gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
			gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
		}
	}
}

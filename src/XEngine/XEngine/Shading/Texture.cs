using System;
using System.Collections.Generic;

using SharpGL;

namespace XEngine.Shading
{
	public abstract class Texture : IDisposable
	{
		private static readonly Dictionary<uint, Texture> BindingCache = new Dictionary<uint, Texture>();
		internal static void InvalidateBindingCache() => BindingCache.Clear();
		internal static void InvalidateBinding(uint i) { if (BindingCache.ContainsKey(i)) BindingCache.Remove(i); }

		private readonly uint[] glTextureArray = new uint[1] { 0u };

		public uint TextureId => glTextureArray[0];

		protected abstract uint TextureType { get; }

		protected Texture()
		{
			var gl = XEngineContext.Graphics;
			gl.GenTextures(1, glTextureArray);
		}
		protected Texture(uint id)
		{
			glTextureArray[0] = id;
		}

		public void Activate(uint index = 0u)
		{
			if (BindingCache.TryGetValue(index, out var bound) && this == bound) return;
			var gl = XEngineContext.Graphics;
			gl.ActiveTexture(OpenGL.GL_TEXTURE0 + index);
			gl.BindTexture(TextureType, glTextureArray[0]);
			BindingCache[index] = this;
		}

		public virtual void Dispose()
		{
			if (TextureId == 0) throw new InvalidOperationException("Already disposed."); ;
			var gl = XEngineContext.Graphics;
			gl.DeleteTextures(1, glTextureArray);
			glTextureArray[0] = 0u;
		}
	}
}

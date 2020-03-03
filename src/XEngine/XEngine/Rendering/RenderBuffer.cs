using System;
using SharpGL;

namespace XEngine.Rendering
{
	public sealed class RenderBuffer : IDisposable
	{
		private static uint CurrentBound = 0u;

		private readonly uint[] glRenderBufferArray = new uint[1] { 0u };

		public uint Id => glRenderBufferArray[0];

		public int Width { get; private set; }
		public int Height { get; private set; }

		private bool Prepared = false;

		public RenderBuffer(int width, int height)
		{
			var gl = XEngineContext.Graphics;
			gl.GenRenderbuffersEXT(1u, glRenderBufferArray);

			Width = width;
			Height = height;
		}

		public void Bind()
		{
			if (Id == CurrentBound) return;
			var gl = XEngineContext.Graphics;
			gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, Id);
			CurrentBound = Id;
		}

		public void PrepareAsDepthBuffer()
		{
			if (Prepared) throw new InvalidOperationException("Render buffer already prepared.");

			var gl = XEngineContext.Graphics;
			Bind();
			gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_DEPTH_COMPONENT, Width, Height);
			
			Prepared = true;
		}

		public void Dispose()
		{
			if (Id == 0) throw new InvalidOperationException("Already disposed."); ;

			var gl = XEngineContext.Graphics;
			gl.DeleteRenderbuffersEXT(1u, glRenderBufferArray);
			glRenderBufferArray[0] = 0u;
		}
	}
}

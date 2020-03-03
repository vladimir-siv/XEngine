using System;

using SharpGL;

namespace XEngine.Rendering
{
    using XEngine.Shading;

	public sealed class FrameBuffer : IDisposable
	{
		private static uint CurrentBound = 0u;
		public static bool IsDefaultBound => CurrentBound == 0u;
		public static void BindRenderingWindow()
		{
			var gl = XEngineContext.Graphics;
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0u);
			CurrentBound = 0u;
		}

		private readonly uint[] glFrameBufferArray = new uint[1] { 0u };
		public uint Id => glFrameBufferArray[0];

		public int Width { get; private set; }
		public int Height { get; private set; }

		public Texture TextureAttachment { get; private set; } = null;
		public Texture DepthTextureAttachment { get; private set; } = null;
		public RenderBuffer DepthBufferAttachment { get; private set; } = null;

		public FrameBuffer(int width, int height, FBOAttachment attachments = FBOAttachment.NONE)
		{
			Width = width;
			Height = height;

			Create(attachments);
		}

		private void Create(FBOAttachment attachments)
		{
			var gl = XEngineContext.Graphics;
			gl.GenFramebuffersEXT(1u, glFrameBufferArray);
			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, Id);
			gl.DrawBuffer(OpenGL.GL_COLOR_ATTACHMENT0_EXT);

			if (attachments.HasFlag(FBOAttachment.TextureAttachment)) AddTextureAttachment();
			if (attachments.HasFlag(FBOAttachment.DepthTextureAttachment)) AddDepthTextureAttachment();
			if (attachments.HasFlag(FBOAttachment.DepthBufferAttachment)) AddDepthBufferAttachment();

			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0u);
		}

		private void AddTextureAttachment()
		{
			if (TextureAttachment != null) return;
			var gl = XEngineContext.Graphics;
			var texture = Texture2D.Yield();
			texture.InitEmpty(Width, Height);
			gl.FramebufferTexture(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, texture.TextureId, 0);
			TextureAttachment = texture;
		}
		private void AddDepthTextureAttachment()
		{
			if (DepthTextureAttachment != null) return;
			var gl = XEngineContext.Graphics;
			var texture = Texture2D.Yield();
			texture.InitDepth(Width, Height);
			gl.FramebufferTexture(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_DEPTH_ATTACHMENT_EXT, texture.TextureId, 0);
			DepthTextureAttachment = texture;
		}
		private void AddDepthBufferAttachment()
		{
			if (DepthBufferAttachment != null) return;
			var gl = XEngineContext.Graphics;
			var buffer = new RenderBuffer(Width, Height);
			buffer.PrepareAsDepthBuffer();
			gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_DEPTH_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT, buffer.Id);
			DepthBufferAttachment = buffer;
		}

		public void Bind()
		{
			var gl = XEngineContext.Graphics;

			Texture.InvalidateBinding(0u);
			gl.ActiveTexture(OpenGL.GL_TEXTURE0 + 0u);
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0u);

			if (Id == CurrentBound) return;

			gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, Id);
			gl.Viewport(0, 0, Width, Height);

			CurrentBound = Id;
		}
		public void Unbind()
		{
			if (Id == CurrentBound) BindRenderingWindow();
		}

		public void Dispose()
		{
			if (Id == 0) throw new InvalidOperationException("Already disposed."); ;
			
			var gl = XEngineContext.Graphics;

			TextureAttachment?.Dispose();
			TextureAttachment = null;

			DepthTextureAttachment?.Dispose();
			DepthTextureAttachment = null;

			DepthBufferAttachment?.Dispose();
			DepthBufferAttachment = null;

			gl.DeleteFramebuffersEXT(1u, glFrameBufferArray);
			glFrameBufferArray[0] = 0u;
		}
	}

	[Flags] public enum FBOAttachment
	{
		NONE = 0,
		TextureAttachment = 1,
		DepthBufferAttachment = 2,
		DepthTextureAttachment = 4
	}
}

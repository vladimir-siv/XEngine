using System;

namespace XEngine.Rendering
{
	public sealed class WaterFrameBuffers : IDisposable
	{
		private const int REFLECTION_WIDTH = 320;
		private const int REFLECTION_HEIGHT = 180;

		private const int REFRACTION_WIDTH = 1280;
		private const int REFRACTION_HEIGHT = 720;

		public FrameBuffer ReflectionFBO { get; private set; }
		public FrameBuffer RefractionFBO { get; private set; }

		public WaterFrameBuffers()
		{
			ReflectionFBO = new FrameBuffer
			(
				REFLECTION_WIDTH,
				REFLECTION_HEIGHT,
				FBOAttachment.TextureAttachment// | FBOAttachment.DepthBufferAttachment
			);

			RefractionFBO = new FrameBuffer
			(
				REFRACTION_WIDTH,
				REFRACTION_HEIGHT,
				FBOAttachment.TextureAttachment// | FBOAttachment.DepthBufferAttachment
			);
		}

		public void Dispose()
		{
			ReflectionFBO?.Dispose();
			RefractionFBO?.Dispose();

			ReflectionFBO = null;
			RefractionFBO = null;
		}
	}
}

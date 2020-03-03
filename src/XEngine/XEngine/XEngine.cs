using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SharpGL;

namespace XEngine
{
	using XEngine.Core;
	using XEngine.Shading;
	using XEngine.Lighting;
	using XEngine.Interaction;

    public static class XEngineActivator
	{
		public static void InitEngine(OpenGLControl control)
		{
			if (XEngineContext.GLControl != null) return;
			XEngineContext.GLControl = control;

			Input.Init();

			foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass))
			{
				var engActivateAttr = type.GetCustomAttributes(typeof(XEngineActivationAttribute), false);
				if (engActivateAttr.Length == 0) continue;
				var attr = (XEngineActivationAttribute)engActivateAttr[0];
				type.GetMethod(attr.MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
			}

			XEngineContext.SkyboxShader = Shader.CreateInternal("skybox");

			Time.Init();

			SceneManager.LoadScene(SceneManager.MainSceneId);
		}

		public static void Shutdown()
		{
			if (XEngineContext.GLControl == null) return;
			foreach (var scene in Scene.SceneCache) scene.Value._Exit();
			XEngineContext.Graphics.UseProgram(0);
			foreach (var shader in XEngineContext.Shaders) shader.Value.Clean();
			XEngineContext.Shaders.Clear();
			foreach (var texture in XEngineContext.Textures) texture.Value.Dispose();
			XEngineContext.Textures.Clear();
			Texture.InvalidateBindingCache();
			XEngineContext.SkyboxShader.Dispose();
			Skybox.mesh = null;
			XEngineContext.GLControl = null;
		}
	}

	public static class XEngineContext
	{
		internal static OpenGLControl GLControl { get; set; } = null;
		internal static OpenGL Graphics => GLControl.OpenGL;

		internal static Dictionary<string, Shader> Shaders { get; } = new Dictionary<string, Shader>();
		internal static Dictionary<string, Texture> Textures { get; } = new Dictionary<string, Texture>();

		internal static Shader SkyboxShader { get; set; } = null;

		public static void Draw()
		{
			Input.Update();
			Time.Update();
			SceneManager.CurrentScene._Draw();
			Input.Late();
		}
	}

	public static class XEngineState
	{
		private static bool _DepthTest = false;
		public static bool DepthTest
		{
			get
			{
				return _DepthTest;
			}
			set
			{
				if (value == _DepthTest) return;
				_DepthTest = value;
				var gl = XEngineContext.Graphics;
				if (_DepthTest) gl.Enable(OpenGL.GL_DEPTH_TEST);
				else gl.Disable(OpenGL.GL_DEPTH_TEST);
			}
		}

		private static bool _CullFace = false;
		public static bool CullFace
		{
			get
			{
				return _CullFace;
			}
			set
			{
				if (value == _CullFace) return;
				_CullFace = value;
				var gl = XEngineContext.Graphics;
				if (_CullFace) gl.Enable(OpenGL.GL_CULL_FACE);
				else gl.Disable(OpenGL.GL_CULL_FACE);
			}
		}

		private static bool _Texture2D = false;
		public static bool Texture2D
		{
			get
			{
				return _Texture2D;
			}
			set
			{
				if (value == _Texture2D) return;
				_Texture2D = value;
				var gl = XEngineContext.Graphics;
				if (_Texture2D) gl.Enable(OpenGL.GL_TEXTURE_2D);
				else gl.Disable(OpenGL.GL_TEXTURE_2D);
			}
		}

		private static bool _ClipDistance = false;
		public static bool ClipDistance
		{
			get
			{
				return _ClipDistance;
			}
			set
			{
				if (value == _ClipDistance) return;
				_ClipDistance = value;
				var gl = XEngineContext.Graphics;
				if (_ClipDistance) gl.Enable(OpenGL.GL_CLIP_DISTANCE0);
				else gl.Disable(OpenGL.GL_CLIP_DISTANCE0);
			}
		}

		private static bool _Blending = false;
		public static bool Blending
		{
			get
			{
				return _Blending;
			}
			set
			{
				if (value == _Blending) return;
				_Blending = value;
				var gl = XEngineContext.Graphics;
				if (_Blending) { gl.Enable(OpenGL.GL_BLEND); gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA); }
				else { gl.Disable(OpenGL.GL_BLEND); }
			}
		}

		private static Color _ClearColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
		public static Color ClearColor
		{
			get
			{
				return _ClearColor;
			}
			set
			{
				if (Color.AreEqual(value, _ClearColor)) return;
				_ClearColor = value;
				var gl = XEngineContext.Graphics;
				gl.ClearColor(value.r, value.g, value.b, value.a);
			}
		}

		public static void Reset()
		{
			DepthTest = true;
			CullFace = true;
			Texture2D = true;
			ClipDistance = true;
			Blending = false;
		}
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class XEngineActivationAttribute : Attribute
	{
		public string MethodName { get; }
		public XEngineActivationAttribute(string methodName) => MethodName = methodName;
	}
}

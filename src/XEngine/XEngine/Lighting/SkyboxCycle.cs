using System;

using SharpGL;
using GlmNet;

namespace XEngine.Lighting
{
	using XEngine.Core;
	using XEngine.Structures;
	using XEngine.Shading;
	using XEngine.Common;

    public class SkyboxCycle
	{
		private readonly Queue<Skybox> Cycle = new Queue<Skybox>();
		public bool CycleMode { get; set; } = false;
		public int SkyCount => Cycle.Count;

		public float Scale { get; set; } = 750.0f;
		public float Rotation { get; set; } = 0.0f;
		public float Transition { get; set; } = 0.0f;

		private mat4 transform = mat4.identity();
		private readonly float[] transform_cache = new float[16];

		public Color SkyColor
		{
			get
			{
				if (Cycle.Count == 0) return Color.Black;
				var sky1 = Cycle.Peek().SkyColor;
				var sky2 = Cycle.Count == 1 ? sky1 : Cycle.Second().SkyColor;
				return algebra.lerp(sky1, sky2, Transition);
			}
		}

		public void Add(Skybox skybox)
		{
			if (CycleMode) throw new InvalidOperationException("Cannot add a skybox while cycle mode is enabled.");
			if (skybox.texture == null) throw new ArgumentException("Cannot add ");
			Cycle.Enqueue(skybox);
		}
		public void Clear()
		{
			if (CycleMode) throw new InvalidOperationException("Cannot add a skybox while cycle mode is enabled.");
			Cycle.Clear();
		}

		public void Swap()
		{
			if (Cycle.Count == 0) throw new ApplicationException("Skybox cycle is empty.");
			Cycle.Enqueue(Cycle.Dequeue());
		}

		internal void Draw()
		{
			if (!CycleMode) return;
			if (Cycle.Count == 0) throw new ApplicationException("Skybox cycle is empty.");

			var gl = XEngineContext.Graphics;
			var shader = XEngineContext.SkyboxShader;
			var scene = SceneManager.CurrentScene;
			var camera = scene.MainCamera;

			transform = camera.WorldToView.copy_to(transform);
			transform = quaternion.euler(transform, 0.0f, Rotation, 0.0f);
			transform.serialize(transform_cache);
			transform_cache[12] = transform_cache[13] = transform_cache[14] = 0.0f;

			shader.Use();

			gl.UniformMatrix4(shader.Project, 1, false, camera.ViewToProjectData);
			gl.UniformMatrix4(shader.View, 1, false, transform_cache);
			shader.SetScalar("scale", Scale);
			shader.SetScalar("transition", Transition);

			var sky1 = Cycle.Peek();
			var sky2 = Cycle.Count == 1 ? sky1 : Cycle.Second();

			sky1.texture.Activate(0u);
			sky2.texture.Activate(1u);

			shader.SetVec4("sky1_color", sky1.SkyColor.r, sky1.SkyColor.g, sky1.SkyColor.b, sky1.SkyColor.a);
			shader.SetScalar("sky1_map", 0);
			shader.SetVec4("sky2_color", sky2.SkyColor.r, sky2.SkyColor.g, sky2.SkyColor.b, sky2.SkyColor.a);
			shader.SetScalar("sky2_map", 1);

			Skybox.mesh.Activate();
			gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, Skybox.SkyboxShape.Geometry.VertexCount);
		}
	}
}

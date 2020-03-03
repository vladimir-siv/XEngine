using System;
using System.Collections.Generic;
using SharpGL;
using GlmNet;

namespace XEngine.Core
{
	using XEngine.Structures;
	using XEngine.Rendering;
	using XEngine.Shading;
	using XEngine.Lighting;
	using XEngine.Common;

	public abstract class Scene
	{
		private class Algorithms
		{
			public int LightingState { get; private set; } = 0;
			public uint LightCount { get; private set; } = 0u;

			public readonly Heap<float, LightSource> LightsHeap = new Heap<float, LightSource>(float.NegativeInfinity);
			public readonly List<LightSource> OrderedLights = new List<LightSource>(8);
			public readonly Pouch<GameObject, GameObject> SyncObjectPouch = new Pouch<GameObject, GameObject>();
			public readonly Queue<GameObject> ObjectQueue = new Queue<GameObject>();
			public readonly Pouch3L<Shader, Mesh, Material, GameObject> DrawableObjectPouch = new Pouch3L<Shader, Mesh, Material, GameObject>();

			public void Invalidate()
			{
				LightingState = 0;
				LightCount = 0u;
			}

			public void UpdateLights(IEnumerable<LightSource> lights, vec3 camera, uint count)
			{
				var change = false;

				LightCount = 0u;
				LightsHeap.Clear(); // Generates garbage

				foreach (var light in lights)
				{
					LightsHeap.Insert(light ^ camera, light);
				}

				for (LightCount = 0u; LightCount < count; ++LightCount)
				{
					if (LightsHeap.Count == 0) break;

					var i = (int)LightCount;

					var light = LightsHeap.RemoveMin().Data;

					if (i < OrderedLights.Count)
					{
						if (LightSource.AreEqual(light, OrderedLights[i])) continue;
						OrderedLights[i] = light;
					}
					else OrderedLights.Add(light);

					change = true;
				}

				if (change) ++LightingState;
			}
			public IEnumerable<GameObject> SyncLevelOrder(GameObject gameObject)
			{
				ObjectQueue.Enqueue(gameObject);
				while (ObjectQueue.Count > 0)
				{
					var current = ObjectQueue.Dequeue();
					yield return current;
					while (SyncObjectPouch.Retrieve(current, out var child)) ObjectQueue.Enqueue(child);
				}
			}

			public void OnLightingStateChange() => ++LightingState;
		}

		internal static Dictionary<string, Scene> SceneCache = new Dictionary<string, Scene>();
		public static Scene Resolve(string sceneId) => SceneCache[sceneId];

		public string SceneId { get; internal set; } = string.Empty;
		protected bool Initialized { get; private set; } = false;
		protected uint DrawCalls { get; set; } = 0u;
		private DateTime InitTime;
		public float ElapsedTime => (float)(DateTime.Now - InitTime).TotalMilliseconds;

		protected uint ClearStrategy { get; set; }
		private readonly Algorithms Algs = new Algorithms();

		protected internal bool ClipDistance { get; protected set; }
		private vec4 _ClipPlane;
		protected internal vec4 ClipPlane
		{
			get
			{
				if (ClipDistance) return _ClipPlane;
				else return new vec4(0.0f, 0.0f, 0.0f, 0.0f);
			}
			protected set
			{
				_ClipPlane = value;
			}
		}

		public Camera MainCamera { get; private set; } = new Camera();
		internal int CameraState { get; private set; }
		protected void UpdateCamera() => ++CameraState;

		public Sky Sky { get; private set; }

		public uint ActiveLights { get; set; }
		internal readonly LinkedList<LightSource> Lights = new LinkedList<LightSource>();
		internal readonly Dictionary<string, float> Intensity = new Dictionary<string, float>();
		internal int LightingState => Algs.LightingState;
		internal uint LightCount => Algs.LightCount;
		internal LightSource GetLight(int i)
		{
			if (i >= ActiveLights) throw new ApplicationException("Invalid light requested.");
			
			if (i < Algs.LightCount)
			{
				var light = Algs.OrderedLights[i];

				if
				(
					light.name != null
					&&
					Intensity.TryGetValue(light.name, out var intensity)
				)
					light.power *= intensity.Clamp(0.0f, 100.0f) / 100.0f;
				
				return light;
			}
			else return LightSource.PitchBlack;
		}
		public void SetLightIntensity(string name, float intensity)
		{
			Intensity[name] = intensity;
			Algs.OnLightingStateChange();
		}
		public void ClearLightIntensity(string name)
		{
			Intensity.Remove(name);
			Algs.OnLightingStateChange();
		}
		
		internal readonly LinkedList<GameObject> GameObjects = new LinkedList<GameObject>();

		public void Add(LightSource lightSource)
		{
			Lights.AddLast(lightSource);
		}
		public void ClearLights()
		{
			Lights.Clear();
		}

		public void Add(GameObject gameObject)
		{
			GameObjects.AddLast(gameObject);
			
			if (Initialized)
			{
				gameObject.Awake();
				gameObject.Start();
			}
		}
		public void ClearGameObjects()
		{
			foreach (var gameObject in GameObjects) gameObject.Dispose();
			GameObjects.Clear();
		}

		public void Clear()
		{
			ClearGameObjects();
			ClearLights();
		}
		
		private void Invalidate()
		{
			var gl = XEngineContext.Graphics;

			ClearStrategy = OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT;
			Algs.Invalidate();

			ClipDistance = false;
			ClipPlane = new vec4(0.0f, 0.0f, 0.0f, 0.0f);
			
			MainCamera.LocalPosition = vector3.zero;
			MainCamera.LocalRotation = vector3.zero;
			CameraState = 0;

			Sky = new Sky();

			gl.ClearColor
			(
				Sky.StaticSkybox.SkyColor.r,
				Sky.StaticSkybox.SkyColor.g,
				Sky.StaticSkybox.SkyColor.b,
				Sky.StaticSkybox.SkyColor.a
			);

			ActiveLights = 8u;

			foreach (var shader in XEngineContext.Shaders) shader.Value.Invalidate();
		}

		internal void _Init()
		{
			if (Initialized) return;
			XEngineState.Reset();
			Invalidate();
			Init();
			if (Lights.Count == 0) Add(LightSource.Sun);
			Sky.BeginCycle();
			InitTime = DateTime.Now;
			Initialized = true;
			foreach (var gameObject in GameObjects) gameObject.Awake();
			foreach (var gameObject in GameObjects) gameObject.Start();
		}
		internal void _Draw()
		{
			if (!Initialized) return;
			MainCamera.AspectRatio = (float)XEngineContext.GLControl.Width / (float)XEngineContext.GLControl.Height;
			foreach (var gameObject in GameObjects) gameObject.Update();
			foreach (var gameObject in GameObjects) gameObject.Late();
			var gl = XEngineContext.Graphics;
			if (ClearStrategy != 0u) gl.Clear(ClearStrategy);
			Draw();
		}
		internal void _Exit()
		{
			if (!Initialized) return;
			Clear();
			Exit();
			Initialized = false;
		}

		protected virtual void Init() { }
		protected virtual void Draw() { DrawCalls = 1u; Prepare(); SyncScene(); Viewport(); DrawScene(); }
		protected virtual void Exit() { }

		protected void Prepare()
		{
			foreach (var gameObject in GameObjects)
			{
				if (gameObject.parent != null) Algs.SyncObjectPouch.Add(gameObject.parent, gameObject);
				if (gameObject.IsDrawable && !gameObject.DisableRendering) Algs.DrawableObjectPouch.Add(gameObject.material.shader, gameObject.mesh, gameObject.material, gameObject);
			}
		}
		protected void SyncScene()
		{
			foreach (var gameObject in GameObjects)
			{
				if (gameObject.parent != null) continue;
				
				foreach (var obj in Algs.SyncLevelOrder(gameObject))
				{
					obj.Sync();
				}
			}

			MainCamera.Adjust(); ++CameraState;

			Sky.Update();
			Algs.UpdateLights(Lights, MainCamera.Position, ActiveLights);
		}
		protected void Viewport()
		{
			if (FrameBuffer.IsDefaultBound)
			{
				var gl = XEngineContext.Graphics;
				gl.Viewport(0, 0, XEngineContext.GLControl.Width, XEngineContext.GLControl.Height);
			}
		}
		protected void DrawScene()
		{
			if (DrawCalls > 0u)
			{
				--DrawCalls;

				Sky.Draw();

				foreach (var gameObject in Algs.DrawableObjectPouch.Redeem(DrawCalls == 0u))
				{
					gameObject.Draw();
				}
			}
		}
	}
}

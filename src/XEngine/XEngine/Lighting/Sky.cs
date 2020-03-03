using System;

namespace XEngine.Lighting
{
	using XEngine.Core;
	using XEngine.Shading;

	public class Sky
	{
		internal int AmbientState { get; private set; } = 0;

		public SkyboxCycle Cycle { get; } = new SkyboxCycle();

		private Skybox _StaticSkybox = Skybox.Default;
		public Skybox StaticSkybox
		{
			get
			{
				return _StaticSkybox;
			}
			set
			{
				if (value.Id == _StaticSkybox.Id) return;
				if (value.texture != null) throw new ArgumentException("Static skybox must be plan.");
				_StaticSkybox = value;
				var color = value.SkyColor;
				var gl = XEngineContext.Graphics;
				gl.ClearColor(color.r, color.g, color.b, color.a);
				++AmbientState;
			}
		}

		private AmbientLight _AmbientLight = AmbientLight.Bright;
		public AmbientLight AmbientLight
		{
			get
			{
				return _AmbientLight;
			}
			set
			{
				if (AmbientLight.AreEqual(value, _AmbientLight)) return;
				_AmbientLight = value;
				++AmbientState;
			}
		}

		private float _FogDensity = 0.005f;
		public float FogDensity
		{
			get
			{
				return _FogDensity;
			}
			set
			{
				if (value == _FogDensity) return;
				_FogDensity = value;
				++AmbientState;
			}
		}
		private float _FogGradient = 10f;
		public float FogGradient
		{
			get
			{
				return _FogGradient;
			}
			set
			{
				if (value == _FogGradient) return;
				_FogGradient = value;
				++AmbientState;
			}
		}

		public Color SkyColor
		{
			get
			{
				if (Cycle.SkyCount == 0) return StaticSkybox.SkyColor;
				return Cycle.SkyColor;
			}
		}

		public float RotationSpeed { get; set; } = 0.5f;
		public float TransitionSpeed { get; set; } = 0.1f;
		public float SkyboxDuration { get; set; } = 10.0f;
		public float TransitionDuration { get; set; } = 2.0f;
		private float RemainingDuration = 0.0f;

		public void BeginCycle()
		{
			if (Cycle.SkyCount == 0) return;
			if (Cycle.CycleMode) return;
			RemainingDuration = SkyboxDuration;
			Cycle.CycleMode = true;
		}
		public void EndCycle()
		{
			if (!Cycle.CycleMode) return;
			Cycle.CycleMode = false;
		}

		internal void Update()
		{
			if (!Cycle.CycleMode) return;
			if (TransitionSpeed < 0.0f) throw new ApplicationException("Transition speed cannot be negative.");
			var deltaTime = Time.DeltaTime / 1000.0f;
			Cycle.Rotation += RotationSpeed * deltaTime;
			var delta = TransitionSpeed * deltaTime;
			RemainingDuration -= delta;
			if (RemainingDuration >= 0.0f) return;
			Cycle.Transition += delta;
			++AmbientState;
			if (Cycle.Transition <= TransitionDuration) return;
			Cycle.Transition = 0.0f;
			Cycle.Swap();
			RemainingDuration = SkyboxDuration;
		}
		internal void Draw()
		{
			if (!Cycle.CycleMode) return;
			Cycle.Draw();
		}
	}
}

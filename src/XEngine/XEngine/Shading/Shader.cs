using System;
using System.Collections.Generic;
using System.Text;
using SharpGL;

namespace XEngine.Shading
{
	using XEngine.Core;

	public sealed class Shader : IDisposable
	{
		public struct LightSourceLocations
		{
			public static readonly LightSourceLocations Empty = new LightSourceLocations(-1, -1, -1, -1);

			public int position;
			public int color;
			public int power;
			public int attenuation;

			public bool Valid => position != -1 && color != -1 && power != -1 && attenuation != -1;

			public LightSourceLocations(int position, int color, int power, int attenuation)
			{
				this.position = position;
				this.color = color;
				this.power = power;
				this.attenuation = attenuation;
			}
		}

		private static (string, string) Preprocess(string code)
		{
			int LineCount(string str) { return str.Length - str.Replace("\n", string.Empty).Length; }

			const string PRAGMA = "#pragma shader ";

			var shader_part1 = code.IndexOf(PRAGMA);
			if (shader_part1 == -1) throw new FormatException("No shader parts found.");
			var shader_part2 = code.IndexOf(PRAGMA, shader_part1 + PRAGMA.Length);
			if (shader_part2 == -1) throw new FormatException("Only one shader part found.");
			var shader_part3 = code.IndexOf(PRAGMA, shader_part2 + PRAGMA.Length);
			if (shader_part3 != -1) throw new FormatException("More than 2 shader parts found.");

			var part1pragma = shader_part1 + PRAGMA.Length;
			var part1start = code.IndexOf('\n', part1pragma);
			var part1type = code.Substring(part1pragma, part1start - part1pragma).Trim().ToLower();
			var part1 = code.Substring(part1start + 1, shader_part2 - (part1start + 1));

			var part2pragma = shader_part2 + PRAGMA.Length;
			var part2start = code.IndexOf('\n', part2pragma);
			var part2type = code.Substring(part2pragma, part2start - part2pragma).Trim().ToLower();
			var part2 = code.Substring(part2start + 1);

			if (part1type != "vertex" && part1type != "fragment") throw new FormatException($"First shader type unknown. [{part1type}]");
			if (part2type != "vertex" && part2type != "fragment") throw new FormatException($"Second shader type unknown. [{part2type}]");

			if (part1type == part2type) throw new FormatException("Both first and second shader types are the same.");

			var part1lineshift = LineCount(code.Substring(0, shader_part1)) + 1;
			var part2lineshift = LineCount(part1) + 1;
			var sb = new StringBuilder();
			for (var i = 0; i < part1lineshift; ++i) sb.Append('\n');
			part1 = sb.ToString() + part1;
			for (var i = 0; i < part2lineshift; ++i) sb.Append('\n');
			part2 = sb.ToString() + part2;

			var vertex_shader = part1type == "vertex" ? part1 : part2;
			var fragment_shader = part1type == "fragment" ? part1 : part2;

			return (vertex_shader, fragment_shader);
		}
		private static uint Build(string code)
		{
			var gl = XEngineContext.Graphics;

			string GetCompileError(uint shaderId)
			{
				var compileStatus = new int[1];
				gl.GetShader(shaderId, OpenGL.GL_COMPILE_STATUS, compileStatus);
				if (compileStatus[0] != OpenGL.GL_TRUE)
				{
					var infoLogLength = new int[1];
					gl.GetShader(shaderId, OpenGL.GL_INFO_LOG_LENGTH, infoLogLength);
					var buffer = new StringBuilder(infoLogLength[0]);
					gl.GetShaderInfoLog(shaderId, infoLogLength[0], IntPtr.Zero, buffer);
					return buffer.ToString();
				}

				return null;
			}
			string GetLinkError(uint progId)
			{
				var linkStatus = new int[1];
				gl.GetProgram(progId, OpenGL.GL_LINK_STATUS, linkStatus);
				if (linkStatus[0] != OpenGL.GL_TRUE)
				{
					var infoLogLength = new int[1];
					gl.GetProgram(progId, OpenGL.GL_INFO_LOG_LENGTH, infoLogLength);
					var buffer = new StringBuilder(infoLogLength[0]);
					gl.GetProgramInfoLog(progId, infoLogLength[0], IntPtr.Zero, buffer);
					return buffer.ToString();
				}

				return null;
			}

			var shaderCode = Preprocess(code);
			var shaderProgramId = gl.CreateProgram();

			var vertexShaderId = gl.CreateShader(OpenGL.GL_VERTEX_SHADER);
			var fragmentShaderId = gl.CreateShader(OpenGL.GL_FRAGMENT_SHADER);

			gl.ShaderSource(vertexShaderId, shaderCode.Item1);
			gl.ShaderSource(fragmentShaderId, shaderCode.Item2);

			gl.CompileShader(vertexShaderId);
			var vertexCompileError = GetCompileError(vertexShaderId);
			if (vertexCompileError != null) throw new ApplicationException($"[VERTEX COMPILE ERROR]\r\n{vertexCompileError}");

			gl.CompileShader(fragmentShaderId);
			var fragmentCompileError = GetCompileError(fragmentShaderId);
			if (fragmentCompileError != null) throw new ApplicationException($"[FRAGMENT COMPILE ERROR]\r\n{fragmentCompileError}");

			gl.AttachShader(shaderProgramId, vertexShaderId);
			gl.AttachShader(shaderProgramId, fragmentShaderId);

			gl.LinkProgram(shaderProgramId);
			gl.ValidateProgram(shaderProgramId);
			var linkError = GetLinkError(shaderProgramId);
			if (linkError != null) throw new ApplicationException($"[LINK ERROR]\r\n{linkError}");

			gl.DeleteShader(vertexShaderId);
			gl.DeleteShader(fragmentShaderId);

			return shaderProgramId;
		}
		private static uint Create(string shaderName, bool lookUpInternally = false)
		{
			var code = (string)null;
			if (lookUpInternally) code = ManifestResourceManager.LoadInternalShader(shaderName);
			else code = ManifestResourceManager.LoadShader(shaderName);
			return Build(code);
		}

		internal static Shader CreateInternal(string shaderName)
		{
			var id = Create(shaderName, true);
			return new Shader(id, shaderName);
		}
		public static Shader Find(string shaderName)
		{
			var shaders = XEngineContext.Shaders;
			var found = shaders.TryGetValue(shaderName, out var shader);
			if (found) return shader;
			var id = Create(shaderName);
			shader = new Shader(id, shaderName);
			shaders.Add(shaderName, shader);
			return shader;
		}

		public static uint CurrentShaderId = 0;

		public uint Id { get; private set; }
		public string Name { get; private set; }

		public int ClipPlane { get; private set; }
		public int Eye { get; private set; }
		public int Project { get; private set; }
		public int View { get; private set; }
		public int Model { get; private set; }
		public int Rotate { get; private set; }
		public int Skybox { get; private set; }
		public int FogDensity { get; private set; }
		public int FogGradient { get; private set; }
		public int AmbientLightColor { get; private set; }
		public int AmbientLightPower { get; private set; }
		public int LightSourceCount { get; private set; }
		public List<LightSourceLocations> LightSources { get; private set; }

		private readonly Dictionary<string, int> Uniforms = new Dictionary<string, int>();

		private int CameraState = 0;
		private int AmbientState = 0;
		private int LightingState = 0;
		private Material Prepared = null;

		private Shader(uint id, string name)
		{
			Id = id;
			Name = name;

			var gl = XEngineContext.Graphics;
			ClipPlane = gl.GetUniformLocation(id, "clip_plane");
			Eye = gl.GetUniformLocation(id, "eye");
			Project = gl.GetUniformLocation(id, "project");
			View = gl.GetUniformLocation(id, "view");
			Model = gl.GetUniformLocation(id, "model");
			Rotate = gl.GetUniformLocation(id, "rotate");
			Skybox = gl.GetUniformLocation(id, "skybox");
			FogDensity = gl.GetUniformLocation(id, "fog_density");
			FogGradient = gl.GetUniformLocation(id, "fog_gradient");
			AmbientLightColor = gl.GetUniformLocation(id, "ambient_light_color");
			AmbientLightPower = gl.GetUniformLocation(id, "ambient_light_power");
			LightSourceCount = gl.GetUniformLocation(id, "light_source_count");

			var lightlocs = GetLightSourceLocations(0);
			if (lightlocs.Valid) LightSources = new List<LightSourceLocations>((int)SceneManager.CurrentScene.ActiveLights) { lightlocs };

			var scene = SceneManager.CurrentScene;
			if (scene == null) return;
			CameraState = scene.CameraState - 1;
			AmbientState = scene.Sky.AmbientState - 1;
			LightingState = scene.LightingState - 1;
		}

		internal void Clean()
		{
			var gl = XEngineContext.Graphics;
			if (CurrentShaderId == Id) gl.UseProgram(0);
			gl.DeleteProgram(Id);
			Id = 0;
		}
		public void Dispose()
		{
			Clean();
			XEngineContext.Shaders.Remove(Name);
		}

		private LightSourceLocations GetLightSourceLocations(int i)
		{
			var gl = XEngineContext.Graphics;
			return new LightSourceLocations
			(
				gl.GetUniformLocation(Id, $"light_source_position[{i}]"),
				gl.GetUniformLocation(Id, $"light_source_color[{i}]"),
				gl.GetUniformLocation(Id, $"light_source_power[{i}]"),
				gl.GetUniformLocation(Id, $"light_source_attenuation[{i}]")
			);
		}
		internal void Invalidate()
		{
			if (CurrentShaderId == Id) CurrentShaderId = 0;
			CameraState = SceneManager.CurrentScene.CameraState - 1;
			AmbientState = SceneManager.CurrentScene.Sky.AmbientState - 1;
			LightingState = SceneManager.CurrentScene.LightingState - 1;
		}

		public void Use()
		{
			if (Id == 0) throw new InvalidOperationException("Shader object was disposed.");
			if (Id == CurrentShaderId) return;
			var gl = XEngineContext.Graphics;
			gl.UseProgram(Id);
			CurrentShaderId = Id;
		}
		public void Update()
		{
			var gl = XEngineContext.Graphics;
			var scene = SceneManager.CurrentScene;
			var clip = scene.ClipPlane;

			if (ClipPlane != -1) gl.Uniform4(ClipPlane, clip.x, clip.y, clip.z, clip.w);

			if (scene.CameraState != CameraState)
			{
				CameraState = scene.CameraState;
				var camera = scene.MainCamera;
				if (Eye != -1) gl.Uniform3(Eye, camera.Position.x, camera.Position.y, camera.Position.z);
				if (Project != -1) gl.UniformMatrix4(Project, 1, false, camera.ViewToProjectData);
				if (View != -1) gl.UniformMatrix4(View, 1, false, camera.WorldToViewData);
			}

			if (AmbientState != scene.Sky.AmbientState)
			{
				AmbientState = scene.Sky.AmbientState;

				var skybox = scene.Sky.SkyColor;
				var ambient = scene.Sky.AmbientLight;

				if (Skybox != -1) gl.Uniform4(Skybox, skybox.r, skybox.g, skybox.b, skybox.a);
				if (FogDensity != -1) gl.Uniform1(FogDensity, scene.Sky.FogDensity);
				if (FogGradient != -1) gl.Uniform1(FogGradient, scene.Sky.FogGradient);
				if (AmbientLightColor != -1) gl.Uniform3(AmbientLightColor, ambient.color.r, ambient.color.g, ambient.color.b);
				if (AmbientLightPower != -1) gl.Uniform1(AmbientLightPower, ambient.power);
			}

			if (LightingState != scene.LightingState)
			{
				LightingState = scene.LightingState;

				if (LightSourceCount != -1) gl.Uniform1(LightSourceCount, scene.LightCount);

				if (LightSources != null)
				{
					for (var i = 0; i < scene.ActiveLights; ++i)
					{
						var light = scene.GetLight(i);
						var lightLocation = LightSourceLocations.Empty;
						if (i < LightSources.Count) lightLocation = LightSources[i];
						else LightSources.Add(lightLocation = GetLightSourceLocations(i));
						if (!lightLocation.Valid) throw new ApplicationException($"Shader '{Name}' does not have a light source with index '{lightLocation}'.");
						gl.Uniform3(lightLocation.position, light.position.x, light.position.y, light.position.z);
						gl.Uniform3(lightLocation.color, light.color.r, light.color.g, light.color.b);
						gl.Uniform1(lightLocation.power, light.power);
						gl.Uniform3(lightLocation.attenuation, light.attenuation.x0, light.attenuation.x1, light.attenuation.x2);
					}
				}
			}
		}
		public bool IsUsing(string uniform)
		{
			if (Uniforms.ContainsKey(uniform)) return true;
			var gl = XEngineContext.Graphics;
			var location = gl.GetUniformLocation(Id, uniform);
			bool uses = location != -1;
			if (uses) Uniforms.Add(uniform, location);
			return uses;
		}
		public int GetLocation(string name)
		{
			if (Id == 0) throw new InvalidOperationException("Shader object was disposed.");
			var gl = XEngineContext.Graphics;
			var found = Uniforms.TryGetValue(name, out var location);
			if (found) return location;
			location = gl.GetUniformLocation(Id, name);
			if (location == -1) throw new ArgumentException($"Unknown uniform '{name}'.");
			Uniforms.Add(name, location);
			return location;
		}

		internal bool PrepareNeeded(Material material, bool markPrepared = false)
		{
			if (material.shader != this) throw new InvalidOperationException("Material is not using this shader.");
			if (CurrentShaderId != Id) throw new InvalidOperationException("Shader not active (call Use() method before this).");
			if (material == null || !material.IsDynamic && material == Prepared) return false;
			if (markPrepared) Prepared = material;
			return true;
		}
		internal void SetScalar(string name, int value) => XEngineContext.Graphics.Uniform1(GetLocation(name), value);
		internal void SetScalar(string name, uint value) => XEngineContext.Graphics.Uniform1(GetLocation(name), value);
		internal void SetScalar(string name, float value) => XEngineContext.Graphics.Uniform1(GetLocation(name), value);
		internal void SetScalarArray(string name, int[] values) => XEngineContext.Graphics.Uniform1(GetLocation(name), values.Length, values);
		internal void SetScalarArray(string name, uint[] values) => XEngineContext.Graphics.Uniform1(GetLocation(name), values.Length, values);
		internal void SetScalarArray(string name, float[] values) => XEngineContext.Graphics.Uniform1(GetLocation(name), values.Length, values);
		internal void SetVec2(string name, float x, float y) => XEngineContext.Graphics.Uniform2(GetLocation(name), x, y);
		internal void SetVec3(string name, float x, float y, float z) => XEngineContext.Graphics.Uniform3(GetLocation(name), x, y, z);
		internal void SetVec4(string name, float x, float y, float z, float w) => XEngineContext.Graphics.Uniform4(GetLocation(name), x, y, z, w);
		internal void SetVec2Array(string name, float[] values) => XEngineContext.Graphics.Uniform2(GetLocation(name), values.Length / 2, values);
		internal void SetVec3Array(string name, float[] values) => XEngineContext.Graphics.Uniform3(GetLocation(name), values.Length / 3, values);
		internal void SetVec4Array(string name, float[] values) => XEngineContext.Graphics.Uniform4(GetLocation(name), values.Length / 4, values);
		internal void SetMat2(string name, float[] value, bool transpose = false) => XEngineContext.Graphics.UniformMatrix2(GetLocation(name), 1, transpose, value);
		internal void SetMat3(string name, float[] value, bool transpose = false) => XEngineContext.Graphics.UniformMatrix3(GetLocation(name), 1, transpose, value);
		internal void SetMat4(string name, float[] value, bool transpose = false) => XEngineContext.Graphics.UniformMatrix4(GetLocation(name), 1, transpose, value);
		internal void SetMat2Array(string name, float[] values, bool transpose = false) => XEngineContext.Graphics.UniformMatrix2(GetLocation(name), values.Length / 4, transpose, values);
		internal void SetMat3Array(string name, float[] values, bool transpose = false) => XEngineContext.Graphics.UniformMatrix3(GetLocation(name), values.Length / 9, transpose, values);
		internal void SetMat4Array(string name, float[] values, bool transpose = false) => XEngineContext.Graphics.UniformMatrix4(GetLocation(name), values.Length / 16, transpose, values);
	}
}

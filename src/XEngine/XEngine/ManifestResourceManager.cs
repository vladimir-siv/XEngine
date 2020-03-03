using System.Reflection;
using System.IO;

namespace XEngine
{
	public static class ManifestResourceManager
	{
		public static string LoadShader(string shaderName) => LoadFile($"Shaders/{shaderName}.glsl");
		public static Stream LoadFromResources(string resource) => Load($"Resources/{resource}");

		public static string LoadFile(string fileName)
		{
			using (var stream = Load(fileName))
			{
				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}
		public static Stream Load(string resourceName)
		{
			var executingAssembly = Assembly.GetExecutingAssembly();
			var pathToDots = resourceName.Replace("\\", ".").Replace("/", ".");
			var location = string.Format("{0}.{1}", executingAssembly.GetName().Name.Replace('-', '_'), pathToDots);
			return executingAssembly.GetManifestResourceStream(location);
		}

		internal static string LoadInternalShader(string shaderName) => LoadInternalFile($"XEngine/BuiltIn/Shaders/{shaderName}.glsl");

		internal static string LoadInternalFile(string fileName)
		{
			using (var stream = LoadInternal(fileName))
			{
				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}
		internal static Stream LoadInternal(string resourceName)
		{
			var thisAssembly = typeof(ManifestResourceManager).Assembly;
			var pathToDots = resourceName.Replace("\\", ".").Replace("/", ".");
			var location = string.Format("{0}.{1}", thisAssembly.GetName().Name.Replace('-', '_'), pathToDots);
			return thisAssembly.GetManifestResourceStream(location);
		}
	}
}

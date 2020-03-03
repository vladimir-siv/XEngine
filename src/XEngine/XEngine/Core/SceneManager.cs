using System;
using System.Linq;
using System.Reflection;

namespace XEngine.Core
{
	public static class SceneManager
	{
		public static string MainSceneId { get; private set; } = null;
		public static Scene CurrentScene { get; private set; } = null;

		public static event Action<Scene, Scene> SceneChanged;

		static SceneManager()
		{
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(Scene).IsAssignableFrom(t)))
			{
				var genSceneAttr = type.GetCustomAttributes(typeof(GenerateSceneAttribute), false);
				if (genSceneAttr.Length == 0) continue;
				var attr = (GenerateSceneAttribute)genSceneAttr[0];
				var scene = (Scene)Activator.CreateInstance(type);
				scene.SceneId = attr.SceneId;
				Scene.SceneCache.Add(scene.SceneId, scene);
				if (attr.IsMain) MainSceneId = scene.SceneId;
			}
		}

		public static void LoadScene(string sceneId) => LoadScene(Scene.Resolve(sceneId));
		public static void LoadScene(Scene scene)
		{
			var LastScene = CurrentScene;
			LastScene?._Exit();
			CurrentScene = scene;
			CurrentScene._Init();
			SceneChanged?.Invoke(LastScene, CurrentScene);
		}
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class GenerateSceneAttribute : Attribute
	{
		public string SceneId { get; }
		public bool IsMain { get; }

		public GenerateSceneAttribute(string sceneId, bool isMain = false)
		{
			SceneId = sceneId;
			IsMain = isMain;
		}
	}
}

using System;

namespace XEngine.Core
{
	public static class Time
	{
		private static DateTime LastTime;

		public static double _DeltaTime { get; private set; } = 0.0;
		public static float DeltaTime { get; private set; } = 0.0f;

		internal static void Init()
		{
			LastTime = DateTime.Now;
			_DeltaTime = 0.0;
		}
		internal static void Update()
		{
			var CurrentTime = DateTime.Now;
			_DeltaTime = (CurrentTime - LastTime).TotalMilliseconds;
			LastTime = CurrentTime;
			DeltaTime = (float)_DeltaTime;
		}
	}
}

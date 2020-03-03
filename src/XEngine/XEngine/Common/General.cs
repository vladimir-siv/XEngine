using System;
using System.Collections.Generic;

namespace XEngine.Common
{
	public static class General
	{
		public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
		{
			@this.TryGetValue(key, out var value);
			return value;
		}

		public static TValue TryGetSafe<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
		{
			if (@this.TryGetValue(key, out var value)) return value;
			else return default;
		}
	}

	public static class RNG
	{
		private static Random rng = new Random();

		public static double Double() => rng.NextDouble();
		public static int Int(int maxValue) => rng.Next(maxValue);
	}
}

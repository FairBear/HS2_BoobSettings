using System.Collections.Generic;

namespace HS2_BoobSettings
{
	public static class Extensions
	{
		public static bool TryGetValue<T>(this Dictionary<string, object> data, string key, out T value, T defaultValue)
		{
			if (data != null && data.TryGetValue(key, out object __value) && __value is T _value)
			{
				value = _value;
				return true;
			}

			value = defaultValue;
			return false;
		}
	}
}

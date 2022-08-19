using System;
using System.Collections.Generic;
using UnityEngine;

namespace Guardian.Utilities
{
	internal class ResourceLoader
	{
		public static Dictionary<string, object> AssetCache = new Dictionary<string, object>();

		public static bool TryGetAsset<T>(string path, out T value)
		{
			path = "file:///" + Application.streamingAssetsPath + "/" + path;
			return TryGet<T>(path, out value);
		}

		public static bool TryGet<T>(string path, out T value)
		{
			if (AssetCache.TryGetValue(path, out var value2))
			{
				value = (T)value2;
				return true;
			}
			if (TryGetRaw<T>(path, out value))
			{
				AssetCache.Add(path, value);
				return true;
			}
			value = default(T);
			return false;
		}

		public static bool TryGetRaw<T>(string path, out T value)
		{
			value = default(T);
			using (WWW wWW = new WWW(path))
			{
				while (!wWW.isDone)
				{
				}
				if (wWW.error != null)
				{
					return false;
				}
				Type typeFromHandle = typeof(T);
				object obj = wWW.bytes;
				if (typeof(MovieTexture).IsAssignableFrom(typeFromHandle))
				{
					obj = wWW.movie;
				}
				else if (typeof(Texture).IsAssignableFrom(typeFromHandle))
				{
					obj = wWW.texture;
				}
				else if (typeof(AudioClip).IsAssignableFrom(typeFromHandle))
				{
					obj = wWW.audioClip;
				}
				else if (typeof(AssetBundle).IsAssignableFrom(typeFromHandle))
				{
					obj = wWW.assetBundle;
				}
				else if (typeof(string).IsAssignableFrom(typeFromHandle))
				{
					obj = wWW.text;
				}
				value = (T)obj;
				return true;
			}
		}
	}
}

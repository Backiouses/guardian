using System;
using System.Collections;
using SimpleJSON;
using UnityEngine;

namespace Guardian.Utilities
{
	internal class Translator
	{
		private static readonly string ApiUrl = "https://translate.googleapis.com/translate_a/single?client=dict-chrome-ex&sl={0}&tl={1}&dt=t&q={2}";

		public static IEnumerator Translate(string text, string langFrom, string langTo, Action<string[]> callback)
		{
			string arg = WWW.EscapeURL(text);
			string url = string.Format(ApiUrl, langFrom, langTo, arg);
			using (WWW www = new WWW(url))
			{
				yield return www;
				if (www.error != null)
				{
					callback(new string[1] { www.error });
					yield break;
				}
				JSONArray asArray = JSON.Parse(www.text).AsArray;
				callback(new string[2]
				{
					asArray[2].Value,
					asArray[0].AsArray[0].AsArray[0].Value
				});
			}
		}

		public static string[] Translate(string text, string langFrom, string langTo)
		{
			string arg = WWW.EscapeURL(text);
			using (WWW wWW = new WWW(string.Format(ApiUrl, langFrom, langTo, arg)))
			{
				while (!wWW.isDone)
				{
				}
				if (wWW.error != null)
				{
					return new string[1] { wWW.error };
				}
				JSONArray asArray = JSON.Parse(wWW.text).AsArray;
				return new string[2]
				{
					asArray[2].Value,
					asArray[0].AsArray[0].AsArray[0].Value
				};
			}
		}
	}
}

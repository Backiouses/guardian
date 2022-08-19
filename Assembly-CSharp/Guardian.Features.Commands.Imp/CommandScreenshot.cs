using System;
using System.Collections;
using System.IO;
using Guardian.Utilities;
using UnityEngine;

namespace Guardian.Features.Commands.Impl
{
	internal class CommandScreenshot : Command
	{
		private string SaveDir = GuardianClient.RootDir + "\\Screenshots";

		public CommandScreenshot()
			: base("screenshot", new string[1] { "ss" }, "[scale]", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			float result = 1f;
			if (args.Length != 0 && float.TryParse(args[0], out result) && result <= 0f)
			{
				result = 1f;
			}
			FengGameManagerMKII.Instance.StartCoroutine(CoTakeScreenshot(result));
		}

		private IEnumerator CoTakeScreenshot(float scale)
		{
			yield return new WaitForEndOfFrame();
			RenderTexture targetTexture = Camera.main.targetTexture;
			RenderTexture active = RenderTexture.active;
			int num = (int)((float)Screen.width * scale);
			int num2 = (int)((float)Screen.height * scale);
			RenderTexture renderTexture = new RenderTexture(num, num2, 24);
			Camera.main.targetTexture = renderTexture;
			RenderTexture.active = renderTexture;
			Camera.main.Render();
			Texture2D texture2D = new Texture2D(num, num2);
			texture2D.ReadPixels(new Rect(0f, 0f, num, num2), 0, 0);
			texture2D.Apply();
			RenderTexture.active = active;
			Camera.main.targetTexture = targetTexture;
			DateTime now = DateTime.Now;
			string text = "SnapShot-" + now.Day + "_" + now.Month + "_" + now.Year + "-" + now.Hour + "_" + now.Minute + "_" + now.Second + ".jpg";
			GameHelper.TryCreateFile(SaveDir, directory: true);
			File.WriteAllBytes(SaveDir + "\\" + text, texture2D.EncodeToJPG(100));
			UnityEngine.Object.DestroyImmediate(texture2D);
			UnityEngine.Object.DestroyImmediate(renderTexture);
		}
	}
}

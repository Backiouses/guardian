using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Guardian.UI
{
	internal class WindowManager
	{
		private static bool IsFullscreen;

		[DllImport("user32.dll")]
		public static extern int GetActiveWindow();

		[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SetWindowTextW")]
		public static extern bool SetWindowTitle(int hWnd, string lpString);

		[DllImport("user32.dll")]
		public static extern void ShowWindow(int hWnd, int nCmdShow);

		public static void HandleWindowFocusEvent(bool hasFocus)
		{
			if (hasFocus)
			{
				if (IsFullscreen)
				{
					IsFullscreen = false;
					ShowWindow(GetActiveWindow(), 1);
					Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen: true);
					GameObject gameObject = GameObject.Find("MainCamera");
					if (gameObject != null)
					{
						IN_GAME_MAIN_CAMERA component = gameObject.GetComponent<IN_GAME_MAIN_CAMERA>();
						component.StartCoroutine(CoMarkHudDirty(component));
					}
				}
			}
			else if (!IsFullscreen && Screen.fullScreen)
			{
				IsFullscreen = true;
				Screen.SetResolution(960, 600, fullscreen: false);
				ShowWindow(GetActiveWindow(), 2);
			}
		}

		private static IEnumerator CoMarkHudDirty(IN_GAME_MAIN_CAMERA mainCamera)
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			mainCamera.needSetHUD = true;
		}
	}
}

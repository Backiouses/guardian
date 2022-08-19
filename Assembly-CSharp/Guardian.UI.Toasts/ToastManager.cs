using Guardian.Utilities;
using UnityEngine;

namespace Guardian.UI.Toasts
{
	internal class ToastManager
	{
		private SynchronizedList<Toast> Toasts = new SynchronizedList<Toast>();

		public void Draw()
		{
			for (int num = Toasts.Count; num > 0; num--)
			{
				int num2 = Toasts.Count - num;
				Toast toast2 = Toasts[num - 1];
				if (75 * num2 > Screen.height)
				{
					break;
				}
				GUILayout.BeginArea(new Rect(Screen.width - 305, 5 + 75 * num2, 300f, 70f), GuiSkins.Box);
				GUILayout.BeginHorizontal();
				GUILayout.Label(toast2.Title.AsBold());
				GUILayout.FlexibleSpace();
				GUILayout.Label(toast2.Timestamp);
				GUILayout.EndHorizontal();
				GUILayout.Label(toast2.Message);
				GUILayout.EndArea();
			}
			long now = GameHelper.CurrentTimeMillis();
			Toasts.RemoveAll((Toast toast) => (float)(now - toast.Time) / 1000f >= toast.TimeToLive);
		}

		public void Add(Toast toast)
		{
			Toasts.Add(toast);
		}
	}
}

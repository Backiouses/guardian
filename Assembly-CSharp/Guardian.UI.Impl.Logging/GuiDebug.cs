using Guardian.Utilities;
using Photon;
using UnityEngine;

namespace Guardian.UI.Impl.Logging
{
	internal class GuiDebug : Gui
	{
		public override void Draw()
		{
			if (!GuardianClient.Properties.ShowLog.Value || Application.loadedLevelName.Equals("SnapShot") || Application.loadedLevelName.Equals("characterCreation"))
			{
				return;
			}
			if (GuardianClient.Properties.DrawDebugBackground.Value)
			{
				GUILayout.BeginArea(new Rect((float)Screen.width - 331f, (float)Screen.height - 255f, 330f, 225f), GuiSkins.Box);
			}
			else
			{
				GUILayout.BeginArea(new Rect((float)Screen.width - 331f, (float)Screen.height - 255f, 330f, 225f));
			}
			GUILayout.FlexibleSpace();
			GuardianClient.Logger.ScrollPosition = GUILayout.BeginScrollView(GuardianClient.Logger.ScrollPosition);
			GUIStyle style = new GUIStyle(GUI.skin.label)
			{
				margin = new RectOffset(0, 0, 0, 0),
				padding = new RectOffset(0, 0, 0, 0),
				border = new RectOffset(0, 0, 0, 0)
			};
			foreach (Logger.Entry entry in GuardianClient.Logger.Entries)
			{
				try
				{
					GUILayout.Label("[" + entry.Timestamp + "] " + entry.ToString(), style);
				}
				catch
				{
				}
			}
			GUILayout.EndScrollView();
			GUILayout.BeginHorizontal();
			if (GuardianClient.Properties.ShowFramerate.Value)
			{
				GUILayout.Label($"{GuardianClient.FpsCounter.FrameCount} FPS");
			}
			if (GuardianClient.Properties.ShowCoordinates.Value)
			{
				string text = "Coordinates Unavailable";
				if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Stop)
				{
					Photon.MonoBehaviour monoBehaviour = null;
					if (IN_GAME_MAIN_CAMERA.Gametype != 0)
					{
						monoBehaviour = ((!PhotonNetwork.player.IsTitan) ? ((Photon.MonoBehaviour)PhotonNetwork.player.GetHero()) : ((Photon.MonoBehaviour)PhotonNetwork.player.GetTitan()));
					}
					else if (FengGameManagerMKII.Instance.Heroes.Count > 0)
					{
						monoBehaviour = FengGameManagerMKII.Instance.Heroes[0];
					}
					if (monoBehaviour != null)
					{
						text = $"X {MathHelper.Floor(monoBehaviour.transform.position.x)} Y {MathHelper.Floor(monoBehaviour.transform.position.y)} Z {MathHelper.Floor(monoBehaviour.transform.position.z)}";
					}
				}
				GUILayout.Label(text);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}
}

using ExitGames.Client.Photon;
using Guardian.Utilities;
using UnityEngine;

public class BTN_START_MULTI_SERVER : MonoBehaviour
{
	private void OnClick()
	{
		string text = GameObject.Find("InputServerName").GetComponent<UIInput>().label.text;
		int maxPlayers = int.Parse(GameObject.Find("InputMaxPlayer").GetComponent<UIInput>().label.text);
		int num = int.Parse(GameObject.Find("InputMaxTime").GetComponent<UIInput>().label.text);
		string selection = GameObject.Find("PopupListMap").GetComponent<UIPopupList>().selection;
		string text2 = (GameObject.Find("CheckboxHard").GetComponent<UICheckbox>().isChecked ? "hard" : ((!GameObject.Find("CheckboxAbnormal").GetComponent<UICheckbox>().isChecked) ? "normal" : "abnormal"));
		string text3 = IN_GAME_MAIN_CAMERA.Lighting.ToString().ToLower();
		string text4 = GameObject.Find("InputStartServerPWD").GetComponent<UIInput>().label.text;
		if (num > 0)
		{
			if (text4.Length > 0)
			{
				text4 = new SimpleAES().Encrypt(text4);
			}
			text = text + "`" + selection + "`" + text2 + "`" + num + "`" + text3 + "`" + text4 + "`" + MathHelper.RandomInt(int.MinValue, int.MaxValue);
			PhotonNetwork.CreateRoom(text, new RoomOptions
			{
				maxPlayers = maxPlayers,
				customRoomProperties = new Hashtable
				{
					{ "Map", selection },
					{
						"Lighting",
						text3.ToUpper()
					}
				}
			}, TypedLobby.Default);
		}
	}
}

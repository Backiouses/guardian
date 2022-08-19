using UnityEngine;

public class BTN_START_SINGLE_GAMEPLAY : MonoBehaviour
{
	private void OnClick()
	{
		string selection = GameObject.Find("PopupListMap").GetComponent<UIPopupList>().selection;
		string selection2 = GameObject.Find("PopupListCharacter").GetComponent<UIPopupList>().selection;
		IN_GAME_MAIN_CAMERA.Difficulty = (GameObject.Find("CheckboxHard").GetComponent<UICheckbox>().isChecked ? 1 : (GameObject.Find("CheckboxAbnormal").GetComponent<UICheckbox>().isChecked ? 2 : 0));
		IN_GAME_MAIN_CAMERA.Gametype = GameType.Singleplayer;
		IN_GAME_MAIN_CAMERA.SingleCharacter = selection2.ToUpper();
		Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode == CameraType.TPS;
		Screen.showCursor = false;
		if (selection == "trainning_0")
		{
			IN_GAME_MAIN_CAMERA.Difficulty = -1;
		}
		FengGameManagerMKII.Level = LevelInfo.GetInfo(selection);
		Application.LoadLevel(FengGameManagerMKII.Level.Map);
	}
}

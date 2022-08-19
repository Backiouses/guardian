using UnityEngine;

public class PopListCamera : MonoBehaviour
{
	private void Awake()
	{
		if (PlayerPrefs.HasKey("cameraType"))
		{
			GetComponent<UIPopupList>().selection = PlayerPrefs.GetString("cameraType");
		}
	}

	private void OnSelectionChange()
	{
		if (GExtensions.TryParseEnum<CameraType>(GetComponent<UIPopupList>().selection, out var value))
		{
			IN_GAME_MAIN_CAMERA.CameraMode = value;
		}
		PlayerPrefs.SetString("cameraType", GetComponent<UIPopupList>().selection);
	}
}

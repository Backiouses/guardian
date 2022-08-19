using UnityEngine;

public class DaylightChange : MonoBehaviour
{
	private void OnSelectionChange()
	{
		if (GExtensions.TryParseEnum<DayLight>(GetComponent<UIPopupList>().selection, out var value))
		{
			IN_GAME_MAIN_CAMERA.Lighting = value;
		}
	}
}

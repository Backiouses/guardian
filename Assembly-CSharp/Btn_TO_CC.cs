using Guardian;
using Guardian.UI.Impl;
using UnityEngine;

public class Btn_TO_CC : MonoBehaviour
{
	private void OnClick()
	{
		Application.LoadLevel("characterCreation");
		GuardianClient.GuiController.OpenScreen(new GuiCustomCharacter());
	}
}

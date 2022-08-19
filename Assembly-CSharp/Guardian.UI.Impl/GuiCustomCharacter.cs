using UnityEngine;

namespace Guardian.UI.Impl
{
	internal class GuiCustomCharacter : Gui
	{
		private string[] OriginalCharacters = new string[8] { "MIKASA", "LEVI", "PETRA", "ARMIN", "EREN", "MARCO", "JEAN", "SASHA" };

		private CustomCharacterManager _CharacterManager;

		private string hairColorRed = "255";

		private string hairColorGreen = "255";

		private string hairColorBlue = "255";

		public override void Draw()
		{
			if (_CharacterManager != null)
			{
				if (_CharacterManager.setup == null)
				{
					return;
				}
				GUILayout.BeginArea(new Rect(Screen.width / 2 + Screen.width / 4 - 150, Screen.height / 2 + Screen.height / 4 - 150, 300f, 300f), GuiSkins.Box);
				GUILayout.Label("Extended Configuration");
				GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
				GUILayout.Label("Set Stat Preset");
				string[] originalCharacters = OriginalCharacters;
				for (int i = 0; i < originalCharacters.Length; i++)
				{
					HeroStat info = HeroStat.GetInfo(originalCharacters[i]);
					if (info != null && GUILayout.Button(info.Name + " Stats"))
					{
						_CharacterManager.SetStatPoint(CreateStat.Speed, info.Speed);
						_CharacterManager.SetStatPoint(CreateStat.Gas, info.Gas);
						_CharacterManager.SetStatPoint(CreateStat.Blades, info.Blade);
						_CharacterManager.SetStatPoint(CreateStat.Acceleration, info.Accel);
					}
				}
				GUILayout.EndVertical();
				GUILayout.BeginVertical();
				GUILayout.Label("Set Hair R/G/B");
				hairColorRed = GUILayout.TextField(hairColorRed);
				if (GUILayout.Button("Set Red Value") && int.TryParse(hairColorRed, out var result))
				{
					if (result < 0)
					{
						result = 0;
					}
					float num = (float)result / 255f;
					_CharacterManager.hairR.GetComponent<UISlider>().sliderValue = num;
					_CharacterManager.OnHairRChange(num);
				}
				GUILayout.Label(string.Empty);
				hairColorGreen = GUILayout.TextField(hairColorGreen);
				if (GUILayout.Button("Set Green Value") && int.TryParse(hairColorGreen, out var result2))
				{
					if (result2 < 0)
					{
						result2 = 0;
					}
					float num2 = (float)result2 / 255f;
					_CharacterManager.hairG.GetComponent<UISlider>().sliderValue = num2;
					_CharacterManager.OnHairGChange(num2);
				}
				GUILayout.Label(string.Empty);
				hairColorBlue = GUILayout.TextField(hairColorBlue);
				if (GUILayout.Button("Set Blue Value") && int.TryParse(hairColorBlue, out var result3))
				{
					if (result3 < 0)
					{
						result3 = 0;
					}
					float num3 = (float)result3 / 255f;
					_CharacterManager.hairB.GetComponent<UISlider>().sliderValue = num3;
					_CharacterManager.OnHairBChange(num3);
				}
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}
			else
			{
				_CharacterManager = Object.FindObjectOfType<CustomCharacterManager>();
			}
		}
	}
}

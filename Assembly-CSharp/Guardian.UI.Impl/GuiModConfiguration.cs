using System.Collections.Generic;
using System.Text.RegularExpressions;
using Guardian.Features.Properties;
using UnityEngine;

namespace Guardian.UI.Impl
{
	internal class GuiModConfiguration : Gui
	{
		private Regex NumericPattern = new Regex("-?(\\d*\\.?)?\\d+", RegexOptions.IgnoreCase);

		private int Width = 440;

		private int Height = 320;

		private bool ShouldSave;

		private Dictionary<Property, bool> TempBoolProps = new Dictionary<Property, bool>();

		private Dictionary<Property, string> TempIntProps = new Dictionary<Property, string>();

		private Dictionary<Property, string> TempFloatProps = new Dictionary<Property, string>();

		private Dictionary<Property, string> TempStringProps = new Dictionary<Property, string>();

		private List<string> Sections = new List<string>();

		private string CurrentSection = "MC";

		private Vector2 ScrollPosition = new Vector2(0f, 0f);

		public override void OnOpen()
		{
			foreach (Property element in GuardianClient.Properties.Elements)
			{
				string item = element.Name.Substr(0, element.Name.IndexOf("_") - 1);
				if (!Sections.Contains(item))
				{
					Sections.Add(item);
				}
				if (element.Value is bool)
				{
					TempBoolProps.Add(element, (bool)element.Value);
				}
				else if (element.Value is int)
				{
					TempIntProps.Add(element, element.Value.ToString());
				}
				else if (element.Value is float)
				{
					TempFloatProps.Add(element, element.Value.ToString());
				}
				else if (element.Value is string)
				{
					TempStringProps.Add(element, (string)element.Value);
				}
			}
		}

		public override void Draw()
		{
			GUILayout.BeginArea(new Rect(5f, Screen.height - Height - 5, Width, Height), GuiSkins.Box);
			GUILayout.Label("Mod Configuration", GUILayout.Width(Width));
			ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			foreach (string section in Sections)
			{
				if (GUILayout.Button(section, GUILayout.Height(25f)))
				{
					CurrentSection = section;
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Label(CurrentSection.AsBold());
			foreach (Property element in GuardianClient.Properties.Elements)
			{
				if (!element.Name.StartsWith(CurrentSection))
				{
					continue;
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label(element.Name.Substr(CurrentSection.Length + 1, element.Name.Length), GUILayout.MaxWidth(Width / 2));
				GUI.SetNextControlName(element.Name);
				if (element.Value is bool)
				{
					bool flag = TempBoolProps[element];
					TempBoolProps[element] = GUILayout.Toggle(TempBoolProps[element], " " + flag, GUILayout.Width(Width / 2));
				}
				else if (element.Value is int)
				{
					string text = GUILayout.TextField(TempIntProps[element].ToString(), GUILayout.Width(Width / 2));
					if (text.Equals("-"))
					{
						text += "0";
					}
					if (NumericPattern.IsMatch(text))
					{
						TempIntProps[element] = text;
					}
				}
				else if (element.Value is float)
				{
					string text2 = GUILayout.TextField(TempFloatProps[element].ToString(), GUILayout.Width(Width / 2));
					if (text2.Equals("-"))
					{
						text2 += "0";
					}
					else if (text2.StartsWith("-."))
					{
						text2 = "-0" + text2.Substring(1);
					}
					if (NumericPattern.IsMatch(text2))
					{
						TempFloatProps[element] = text2;
					}
				}
				else if (element.Value is string)
				{
					TempStringProps[element] = GUILayout.TextField(TempStringProps[element], GUILayout.Width(Width / 2));
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			if (GUILayout.Button("Save & Close", GUILayout.Height(25f)) || (KeyCode.Escape.IsKeyUp() && GUI.GetNameOfFocusedControl().Length == 0))
			{
				ShouldSave = true;
				GuardianClient.GuiController.OpenScreen(null);
			}
			GUILayout.EndArea();
		}

		public override void OnClose()
		{
			if (!ShouldSave)
			{
				return;
			}
			foreach (KeyValuePair<Property, bool> tempBoolProp in TempBoolProps)
			{
				((Property<bool>)tempBoolProp.Key).Value = tempBoolProp.Value;
			}
			foreach (KeyValuePair<Property, string> tempIntProp in TempIntProps)
			{
				if (int.TryParse(tempIntProp.Value, out var result))
				{
					((Property<int>)tempIntProp.Key).Value = result;
				}
			}
			foreach (KeyValuePair<Property, string> tempFloatProp in TempFloatProps)
			{
				if (float.TryParse(tempFloatProp.Value, out var result2))
				{
					((Property<float>)tempFloatProp.Key).Value = result2;
				}
			}
			foreach (KeyValuePair<Property, string> tempStringProp in TempStringProps)
			{
				((Property<string>)tempStringProp.Key).Value = tempStringProp.Value;
			}
			GuardianClient.Properties.Save();
		}
	}
}

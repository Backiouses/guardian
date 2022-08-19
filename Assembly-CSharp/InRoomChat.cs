using System.Collections.Generic;
using System.Text.RegularExpressions;
using Guardian;
using Guardian.UI;
using Guardian.Utilities;
using Photon;
using UnityEngine;

public class InRoomChat : Photon.MonoBehaviour
{
	public class Message
	{
		public string Sender;

		public string Content;

		public long Time;

		public string Timestamp;

		public Message(string sender, string content)
		{
			Sender = sender;
			Content = content;
			Time = GameHelper.CurrentTimeMillis();
			Timestamp = GameHelper.Epoch.AddMilliseconds(Time).ToLocalTime().ToString("HH:mm:ss");
		}

		public override string ToString()
		{
			if (Sender.Length == 0)
			{
				return Content;
			}
			return Sender + ": " + Content;
		}
	}

	public static InRoomChat Instance;

	public static Rect MessagesRect = new Rect(1f, 0f, 329f, 225f);

	public static Rect ChatBoxRect = new Rect(30f, 575f, 300f, 25f);

	public static List<Message> Messages = new List<Message>();

	private static readonly Regex Detagger = new Regex("<\\/?(color|size|b|i|material|quad)[^>]*>", RegexOptions.IgnoreCase);

	public bool IsVisible = true;

	private bool AlignBottom = true;

	public string inputLine = string.Empty;

	private Vector2 ScrollPosition = GameHelper.ScrollBottom;

	private string TextFieldName = "ChatInput";

	private GUIStyle labelStyle;

	private void Awake()
	{
		Instance = this;
		UpdatePosition();
	}

	public void UpdatePosition()
	{
		if (AlignBottom)
		{
			ScrollPosition = GameHelper.ScrollBottom;
			MessagesRect = new Rect(1f, (float)Screen.height - 255f, 329f, 225f);
			ChatBoxRect = new Rect(30f, (float)Screen.height - 25f, 300f, 25f);
		}
	}

	public void AddLine(string message)
	{
		AddMessage(string.Empty, message);
	}

	public void AddMessage(string sender, string text)
	{
		sender = GuardianClient.BlacklistedTagsPattern.Replace(sender, string.Empty);
		text = GuardianClient.BlacklistedTagsPattern.Replace(text, string.Empty);
		if (sender.Length != 0 || text.Length != 0)
		{
			Messages.Add(new Message(sender, text));
			if (Messages.Count > GuardianClient.Properties.MaxChatLines.Value)
			{
				Messages.RemoveAt(0);
			}
			ScrollPosition = GameHelper.ScrollBottom;
		}
	}

	private void DrawMessageHistory()
	{
		if (labelStyle == null)
		{
			labelStyle = new GUIStyle(GUI.skin.label)
			{
				margin = new RectOffset(0, 0, 0, 0),
				padding = new RectOffset(0, 0, 0, 0)
			};
		}
		if (GuardianClient.Properties.DrawChatBackground.Value)
		{
			GUILayout.BeginArea(MessagesRect, GuiSkins.Box);
		}
		else
		{
			GUILayout.BeginArea(MessagesRect);
		}
		GUILayout.FlexibleSpace();
		ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
		foreach (Message message in Messages)
		{
			try
			{
				string text = message.ToString();
				if (GuardianClient.Properties.ChatTimestamps.Value)
				{
					text = "[" + message.Timestamp + "] " + text;
				}
				GUILayout.Label(text, labelStyle);
				if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type != EventType.Repaint && GUI.GetNameOfFocusedControl().Equals(TextFieldName))
				{
					if (Input.GetMouseButtonDown(0))
					{
						string text2 = Detagger.Replace(message.Content, string.Empty);
						GuardianClient.Commands.Find("translate").Execute(this, new string[3]
						{
							"auto",
							GuardianClient.SystemLanguage,
							text2
						});
					}
					else if (Input.GetMouseButtonDown(1))
					{
						TextEditor textEditor = new TextEditor();
						textEditor.content = new GUIContent(message.Content);
						textEditor.SelectAll();
						textEditor.Copy();
					}
				}
			}
			catch
			{
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void HandleInput()
	{
		if (Event.current == null)
		{
			return;
		}
		KeyCode keyCode = FengGameManagerMKII.InputRC.humanKeys[InputCodeRC.Chat];
		if (Event.current.type == EventType.KeyUp)
		{
			if (keyCode == KeyCode.None || Event.current.keyCode != keyCode || GUI.GetNameOfFocusedControl().Equals(TextFieldName))
			{
				return;
			}
			GUI.FocusControl(TextFieldName);
			inputLine = "\t";
		}
		if (Event.current.type != EventType.KeyDown)
		{
			return;
		}
		if (Event.current.character == '/' && !GUI.GetNameOfFocusedControl().Equals(TextFieldName))
		{
			GUI.FocusControl(TextFieldName);
			inputLine = "/";
		}
		else if (Event.current.character == '\t' && keyCode != KeyCode.Tab && !IN_GAME_MAIN_CAMERA.IsPausing)
		{
			Event.current.Use();
		}
		if (Event.current.keyCode != KeyCode.KeypadEnter && Event.current.keyCode != KeyCode.Return)
		{
			return;
		}
		if (GUI.GetNameOfFocusedControl().Equals(TextFieldName))
		{
			if (!string.IsNullOrEmpty(inputLine) && inputLine != "\t")
			{
				if (FengGameManagerMKII.RCEvents.ContainsKey("OnChatInput"))
				{
					string key = (string)FengGameManagerMKII.RCVariableNames["OnChatInput"];
					if (FengGameManagerMKII.StringVariables.ContainsKey(key))
					{
						FengGameManagerMKII.StringVariables[key] = inputLine;
					}
					else
					{
						FengGameManagerMKII.StringVariables.Add(key, inputLine);
					}
					((RCEvent)FengGameManagerMKII.RCEvents["OnChatInput"]).CheckEvent();
				}
				if (!inputLine.StartsWith("/"))
				{
					string str = GExtensions.AsString(PhotonNetwork.player.customProperties[PhotonPlayerProperty.Name]).NGUIToUnity();
					if (str.StripNGUI().Length < 1)
					{
						str = GExtensions.AsString(PhotonNetwork.player.customProperties[PhotonPlayerProperty.Name]);
					}
					FengGameManagerMKII.Instance.photonView.RPC("Chat", PhotonTargets.All, FormatMessage(inputLine, str));
				}
				else
				{
					GuardianClient.Commands.HandleCommand(this);
				}
			}
			GUI.FocusControl(string.Empty);
			inputLine = string.Empty;
		}
		else
		{
			GUI.FocusControl(TextFieldName);
			inputLine = "\t";
		}
	}

	private void DrawMessageTextField()
	{
		GUILayout.BeginArea(ChatBoxRect);
		GUILayout.BeginHorizontal();
		GUI.SetNextControlName(TextFieldName);
		inputLine = GUILayout.TextField(inputLine, GUILayout.MaxWidth(300f));
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	public void OnGUI()
	{
		if (IsVisible && PhotonNetwork.connected)
		{
			DrawMessageHistory();
			HandleInput();
			DrawMessageTextField();
		}
	}

	public static object[] FormatMessage(string input, string name)
	{
		if (GuardianClient.Properties.TranslateOutgoing.Value)
		{
			string[] array = Translator.Translate(input, GuardianClient.Properties.IncomingLanguage.Value, GuardianClient.Properties.OutgoingLanguage.Value);
			if (array.Length > 1 && !array[0].Equals(GuardianClient.Properties.OutgoingLanguage.Value))
			{
				input = array[1];
			}
		}
		input = input.Replace("<3", "♥");
		input = input.Replace(":lenny:", "( \u0361° \u035cʖ \u0361°)");
		if (input.StripUnityColors().StartsWith(">"))
		{
			input = input.StripUnityColors().AsColor("B5BD68");
		}
		else
		{
			string value = GuardianClient.Properties.TextColor.Value;
			if (value.Length > 0)
			{
				input = input.AsColor(value);
			}
		}
		if (GuardianClient.Properties.BoldText.Value)
		{
			input = input.AsBold();
		}
		if (GuardianClient.Properties.ItalicText.Value)
		{
			input = input.AsItalic();
		}
		string value2 = GuardianClient.Properties.ChatName.Value;
		if (value2.Length != 0)
		{
			name = value2.NGUIToUnity();
		}
		if (GuardianClient.Properties.BoldName.Value)
		{
			name = name.AsBold();
		}
		if (GuardianClient.Properties.ItalicName.Value)
		{
			name = name.AsItalic();
		}
		return new object[2]
		{
			GuardianClient.Properties.TextPrefix.Value + input + GuardianClient.Properties.TextSuffix.Value,
			name
		};
	}
}

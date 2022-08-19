using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.Lite;
using UnityEngine;

public class MicGUI : MonoBehaviour
{
	private Rect micRect;

	private Rect overlayRect;

	private Vector2 vSliderValue;

	private Vector2 controlSlider;

	private float appHeight;

	private int selection;

	private bool guiOn;

	private KeyCode guiKey = KeyCode.Backslash;

	private Rect micAreaRect;

	private float labelLength;

	private Rect micOptionsRect;

	private int changingKeys;

	private GUIStyle overlayStyle;

	private GUIStyle micStyle;

	private GUIStyle areaStyle;

	private GUIStyle buttonStyle;

	private Color buttonGUIColor = new Color(0f, 0.2314f, 0.4588f);

	private bool dropDown;

	private Vector2 clickPos;

	private Rect deviceRect;

	public void Start()
	{
		dropDown = false;
		if (PlayerPrefs.HasKey("voiceKey"))
		{
			guiKey = (KeyCode)PlayerPrefs.GetInt("voiceKey");
		}
		changingKeys = -1;
		selection = 0;
		guiOn = false;
		appHeight = Screen.height;
		AdjustRect();
		overlayStyle = new GUIStyle();
		Texture2D texture2D = new Texture2D(1, 1);
		texture2D.SetPixel(0, 0, new Color(0.1569f, 0.1569f, 0.1569f));
		texture2D.Apply();
		micStyle = new GUIStyle();
		micStyle.normal.background = texture2D;
		Texture2D texture2D2 = new Texture2D(1, 1);
		texture2D2.SetPixel(0, 0, new Color(0.1961f, 0.1961f, 0.1961f));
		texture2D2.Apply();
		areaStyle = new GUIStyle();
		areaStyle.normal.background = texture2D2;
		Texture2D texture2D3 = new Texture2D(1, 1);
		texture2D3.SetPixel(0, 0, buttonGUIColor);
		texture2D3.Apply();
		buttonStyle = new GUIStyle();
		buttonStyle.normal.background = texture2D3;
		Texture2D texture2D4 = new Texture2D(1, 1);
		texture2D4.SetPixel(0, 0, adjustColor(buttonGUIColor, 0.75f));
		texture2D4.Apply();
		buttonStyle.active.background = texture2D4;
		buttonStyle.active.textColor = new Color(0.149f, 0.149f, 0.149f);
		Texture2D texture2D5 = new Texture2D(1, 1);
		texture2D5.SetPixel(0, 0, adjustColor(buttonGUIColor, 1.25f));
		texture2D5.Apply();
		buttonStyle.hover.background = texture2D5;
		buttonStyle.hover.textColor = new Color(0.149f, 0.149f, 0.149f);
		buttonStyle.normal.textColor = Color.white;
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		buttonStyle.wordWrap = true;
	}

	public static Color adjustColor(Color col, float adjustment)
	{
		float r = col.r * adjustment;
		float g = col.g * adjustment;
		float b = col.b * adjustment;
		return new Color(r, g, b);
	}

	public void DrawOverlayGUI(int ID)
	{
		try
		{
			GUILayout.BeginVertical();
			if (MicEF.ThreadId != -1)
			{
				GUILayout.Label("<b>(" + PhotonNetwork.player.Id + ") </b>" + PhotonNetwork.player.customProperties["name"].ToString().NGUIToUnity());
			}
			foreach (KeyValuePair<int, MicPlayer> user in MicEF.Users)
			{
				if (user.Value.Processing)
				{
					GUILayout.Label("<b>(" + user.Key + ") </b>" + user.Value.Name);
				}
			}
			GUILayout.EndVertical();
		}
		catch (Exception message)
		{
			MonoBehaviour.print(message);
		}
	}

	public void DrawMainGUI(int ID)
	{
		GUILayout.BeginVertical();
		GUILayout.BeginArea(micOptionsRect);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("User List", buttonStyle))
		{
			selection = 0;
			dropDown = false;
		}
		else if (GUILayout.Button("Options", buttonStyle))
		{
			selection = 1;
			dropDown = false;
		}
		else if (GUILayout.Button("Credits", buttonStyle))
		{
			selection = 2;
			dropDown = false;
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		GUILayout.BeginArea(micAreaRect, areaStyle);
		switch (selection)
		{
		case 0:
			vSliderValue = GUILayout.BeginScrollView(vSliderValue);
			foreach (KeyValuePair<int, MicPlayer> user in MicEF.Users)
			{
				MicPlayer value = user.Value;
				GUILayout.BeginHorizontal();
				GUILayout.Label(value.Name, GUILayout.Width(labelLength));
				Color textColor2 = buttonStyle.normal.textColor;
				if (value.MutedYou)
				{
					buttonStyle.normal.textColor = Color.yellow;
				}
				else if (!value.isMuted)
				{
					buttonStyle.normal.textColor = Color.green;
				}
				else
				{
					buttonStyle.normal.textColor = Color.red;
				}
				if (GUILayout.Button("M", buttonStyle))
				{
					value.Mute(!value.isMuted);
				}
				buttonStyle.normal.textColor = textColor2;
				if (GUILayout.Button("V", buttonStyle))
				{
					value.changingVolume = !value.changingVolume;
				}
				GUILayout.EndHorizontal();
				if (value.changingVolume)
				{
					value.volume = GUILayout.HorizontalSlider(value.volume, 0f, 4f);
					if (!value.isMuted && value.volume == 0f)
					{
						value.Mute(enabled: true);
					}
					else if (value.isMuted)
					{
						value.Mute(enabled: false);
					}
				}
			}
			GUILayout.EndScrollView();
			break;
		case 1:
		{
			controlSlider = GUILayout.BeginScrollView(controlSlider);
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Push To talk:");
			string text = MicEF.PushToTalk.ToString();
			if (changingKeys == 0)
			{
				text = "Waiting...";
				for (int i = 1; i <= 429; i++)
				{
					KeyCode keyCode = (KeyCode)i;
					if (Input.GetKeyDown(keyCode))
					{
						MicEF.PushToTalk = keyCode;
						changingKeys = -1;
						PlayerPrefs.SetInt("pushToTalk", (int)keyCode);
					}
				}
			}
			if (GUILayout.Button(text, buttonStyle) && changingKeys == -1)
			{
				changingKeys = 0;
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Voice GUI Key:");
			text = guiKey.ToString();
			if (changingKeys == 1)
			{
				text = "Waiting...";
				for (int j = 1; j <= 429; j++)
				{
					KeyCode keyCode2 = (KeyCode)j;
					if (Input.GetKeyDown(keyCode2))
					{
						guiKey = keyCode2;
						changingKeys = -1;
						PlayerPrefs.SetInt("voiceKey", (int)keyCode2);
					}
				}
			}
			if (GUILayout.Button(text, buttonStyle) && changingKeys == -1)
			{
				changingKeys = 1;
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("Volume Multiplier: " + MicEF.VolumeMultiplier);
			float volumeMultiplier = MicEF.VolumeMultiplier;
			MicEF.VolumeMultiplier = GUILayout.HorizontalSlider(MicEF.VolumeMultiplier, 0f, 3f);
			if (volumeMultiplier != MicEF.VolumeMultiplier)
			{
				PlayerPrefs.SetFloat("volumeMultiplier", MicEF.VolumeMultiplier);
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label("Microphone: ");
			string text2 = "Default";
			if (MicEF.DeviceName.Length > 0)
			{
				text2 = MicEF.DeviceName;
				if (text2.StartsWith("Microphone ("))
				{
					text2 = text2.Remove(0, 12);
					text2 = text2.Substring(0, text2.Length - 1);
				}
			}
			if (GUILayout.Button(text2, buttonStyle))
			{
				clickPos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
				deviceRect = new Rect(clickPos.x - micAreaRect.width / 5f, clickPos.y + 5f, micAreaRect.width / 2.5f, micAreaRect.height);
				dropDown = !dropDown;
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Auto Mute People On Join:");
			bool autoMute = MicEF.AutoMute;
			MicEF.AutoMute = GUILayout.Toggle(autoMute, "On");
			if (autoMute != MicEF.AutoMute)
			{
				PlayerPrefs.SetString("voiceAutoMute", MicEF.AutoMute + string.Empty);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Auto Connect:");
			bool autoConnect = MicEF.AutoConnect;
			MicEF.AutoConnect = GUILayout.Toggle(autoConnect, "On");
			if (autoConnect != MicEF.AutoConnect)
			{
				PlayerPrefs.SetString("voiceAutoConnect", MicEF.AutoConnect + string.Empty);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Toggle Mic:");
			bool toggleMic = MicEF.ToggleMic;
			MicEF.ToggleMic = GUILayout.Toggle(toggleMic, "On");
			if (toggleMic != MicEF.ToggleMic)
			{
				PlayerPrefs.SetString("voiceToggleMic", MicEF.ToggleMic + string.Empty);
			}
			GUILayout.EndHorizontal();
			text = "Disconnect From Voice";
			if (MicEF.Disconnected)
			{
				text = "Connect To Voice";
			}
			Color textColor = buttonStyle.normal.textColor;
			if (MicEF.Disconnected)
			{
				buttonStyle.normal.textColor = Color.green;
			}
			else
			{
				buttonStyle.normal.textColor = Color.red;
			}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(text, buttonStyle))
			{
				if (!MicEF.Disconnected)
				{
					PhotonNetwork.RaiseEvent(173, new byte[1] { 253 }, sendReliable: true, new RaiseEventOptions
					{
						Receivers = ReceiverGroup.Others
					});
					MicEF.Disconnected = true;
				}
				else
				{
					PhotonNetwork.RaiseEvent(173, new byte[0], sendReliable: true, new RaiseEventOptions
					{
						Receivers = ReceiverGroup.Others
					});
					MicEF.Disconnected = false;
				}
			}
			buttonStyle.normal.textColor = Color.white;
			if (GUILayout.Button("Reset Settings", buttonStyle))
			{
				MicEF.PushToTalk = KeyCode.V;
				PlayerPrefs.SetInt("pushToTalk", 118);
				guiKey = KeyCode.Backslash;
				PlayerPrefs.SetInt("voiceKey", 92);
				MicEF.DeviceName = string.Empty;
				PlayerPrefs.SetString("micDevice", string.Empty);
				MicEF.VolumeMultiplier = 1f;
				PlayerPrefs.SetFloat("volumeMultiplier", 1f);
				MicEF.AutoMute = false;
				PlayerPrefs.SetString("voiceAutoMute", "false");
				MicEF.AutoConnect = true;
				PlayerPrefs.SetString("voiceAutoConnect", "false");
				MicEF.ToggleMic = false;
				PlayerPrefs.SetString("voiceToggleMic", "false");
				foreach (MicPlayer value2 in MicEF.Users.Values)
				{
					value2.volume = 1f;
					value2.Mute(enabled: false);
				}
			}
			GUILayout.EndHorizontal();
			buttonStyle.normal.textColor = textColor;
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			break;
		}
		case 3:
			GUILayout.Label("Main Developer: Elite Future(Kevin) - Discord:Elite Future#1043");
			GUILayout.Label("Data Compression: Sadico");
			break;
		}
		GUILayout.EndArea();
		GUILayout.EndVertical();
		if ((!Input.GetKey(KeyCode.Mouse0) || !Input.GetKey(KeyCode.Mouse1)) && !Input.GetKey(KeyCode.C) && (IN_GAME_MAIN_CAMERA.CameraMode == CameraType.WOW || IN_GAME_MAIN_CAMERA.CameraMode == CameraType.Original))
		{
			GUI.DragWindow();
		}
	}

	public void DrawDeviceList(int ID)
	{
		GUILayout.BeginVertical();
		string[] devices = Microphone.devices;
		foreach (string text in devices)
		{
			string text2 = text.Remove(0, 12);
			text2 = text2.Substring(0, text2.Length - 1);
			if (GUILayout.Button(text2, buttonStyle))
			{
				MicEF.DeviceName = text;
				dropDown = false;
				PlayerPrefs.SetString("micDevice", text);
			}
		}
		GUILayout.EndVertical();
	}

	public void Update()
	{
		if (Input.GetKeyDown(guiKey) && PhotonNetwork.room != null)
		{
			guiOn = !guiOn;
			dropDown = false;
		}
	}

	public void OnGUI()
	{
		try
		{
			if (PhotonNetwork.room == null)
			{
				return;
			}
			if ((float)Screen.height != appHeight)
			{
				appHeight = Screen.height;
				AdjustRect();
			}
			if (MicEF.Users.Count > 0 || MicEF.ThreadId != -1)
			{
				overlayRect = GUI.Window(1731, overlayRect, DrawOverlayGUI, string.Empty, overlayStyle);
			}
			if (guiOn)
			{
				if (dropDown)
				{
					deviceRect = GUI.Window(1733, deviceRect, DrawDeviceList, string.Empty, overlayStyle);
				}
				micRect = GUI.Window(1732, micRect, DrawMainGUI, string.Empty, micStyle);
			}
		}
		catch (Exception message)
		{
			MonoBehaviour.print(message);
		}
	}

	private void AdjustRect()
	{
		float num = 1920f;
		float num2 = 1080f;
		if (Screen.width > 1920)
		{
			num = Screen.width;
		}
		if (Screen.height > 1080)
		{
			num2 = Screen.height;
		}
		float num3 = num / 4.2f;
		float num4 = num2 / 4.2f;
		overlayRect = new Rect(0f, Screen.height / 2 - 100, 200f, 200f);
		micRect = new Rect((float)Screen.width - num3, (float)Screen.height - num4, num3, num4);
		micAreaRect = new Rect(10f, num4 / 8f, num3 - 20f, num4 / 8f * 7f - 10f);
		micOptionsRect = new Rect(10f, 10f, num3 - 20f, num4 / 8f);
		labelLength = micAreaRect.width / 8f * 6f;
	}
}

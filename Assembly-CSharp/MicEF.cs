using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using UnityEngine;

public class MicEF : MonoBehaviour
{
	private int lastPos;

	private AudioClip clip;

	public static float Frequency = 10000f;

	public static int Decrease = 125;

	public static int ThreadId;

	public static Dictionary<int, MicPlayer> Users;

	public static KeyCode PushToTalk = KeyCode.V;

	public static List<int> MuteList;

	private static int[] SendList;

	public static List<int> AdjustableList;

	public static float VolumeMultiplier = 1f;

	public static bool Disconnected;

	public static bool AutoConnect = false;

	public static string DeviceName = string.Empty;

	public static bool AutoMute = false;

	public static bool ToggleMic = false;

	private bool micToggled;

	public void Start()
	{
		if (PlayerPrefs.HasKey("pushToTalk"))
		{
			PushToTalk = (KeyCode)PlayerPrefs.GetInt("pushToTalk");
		}
		if (PlayerPrefs.HasKey("voiceAutoConnect") && PlayerPrefs.GetString("voiceAutoConnect").ToLower().StartsWith("f"))
		{
			AutoConnect = false;
		}
		if (PlayerPrefs.HasKey("voiceAutoMute") && PlayerPrefs.GetString("voiceAutoMute").ToLower().StartsWith("t"))
		{
			AutoMute = true;
		}
		if (PlayerPrefs.HasKey("voiceToggleMic") && PlayerPrefs.GetString("voiceToggleMic").ToLower().StartsWith("t"))
		{
			ToggleMic = true;
		}
		if (PlayerPrefs.HasKey("volumeMultiplier"))
		{
			VolumeMultiplier = PlayerPrefs.GetFloat("volumeMultiplier");
		}
		if (PlayerPrefs.HasKey("micDevice"))
		{
			DeviceName = PlayerPrefs.GetString("micDevice");
		}
		Disconnected = !AutoConnect;
		SendList = new int[0];
		AdjustableList = new List<int>();
		MuteList = new List<int>();
		Users = new Dictionary<int, MicPlayer>();
		Decrease = (int)(Frequency / 80f);
		ThreadId = -1;
		base.gameObject.AddComponent<MicGUI>();
	}

	public void OnJoinedRoom()
	{
		ThreadId = -1;
		Disconnected = !AutoConnect;
		SendList = new int[0];
		AdjustableList = new List<int>();
		MuteList = new List<int>();
		Users = new Dictionary<int, MicPlayer>();
		if (base.gameObject.GetComponent<MicGUI>() == null)
		{
			base.gameObject.AddComponent<MicGUI>();
		}
	}

	public void OnLeftRoom()
	{
		ThreadId = -1;
		Disconnected = true;
		SendList = new int[0];
		AdjustableList = new List<int>();
		MuteList = new List<int>();
		Users = new Dictionary<int, MicPlayer>();
		if (base.gameObject.GetComponent<MicGUI>() == null)
		{
			base.gameObject.AddComponent<MicGUI>();
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Stop)
		{
			return;
		}
		if (Camera.main.gameObject.GetComponent<AudioSource>() == null)
		{
			Camera.main.gameObject.AddComponent<AudioSource>();
		}
		foreach (KeyValuePair<int, MicPlayer> user in Users)
		{
			user.Value.RefreshInformation();
		}
	}

	public static void AddSpeaker(int id)
	{
		if (!Users.ContainsKey(id))
		{
			Users.Add(id, new MicPlayer(id));
			if (!AdjustableList.Contains(id))
			{
				AdjustableList.Add(id);
				RecompileSendList();
			}
			PhotonNetwork.RaiseEvent(173, new byte[0], sendReliable: true, new RaiseEventOptions
			{
				TargetActors = new int[1] { id }
			});
			if (AutoMute)
			{
				Users[id].Mute(enabled: true);
			}
		}
	}

	public static void RecompileSendList()
	{
		SendList = AdjustableList.ToArray();
	}

	public void Update()
	{
		if (Disconnected)
		{
			return;
		}
		if (ThreadId != -1 && ((!ToggleMic && (Input.GetKeyUp(PushToTalk) || !Input.GetKey(PushToTalk))) || (ToggleMic && micToggled && Input.GetKeyDown(PushToTalk))))
		{
			ThreadId = -1;
			micToggled = false;
		}
		else
		{
			if (!Input.GetKeyDown(PushToTalk) || ThreadId != -1)
			{
				return;
			}
			if (ToggleMic)
			{
				micToggled = true;
			}
			PhotonNetwork.RaiseEvent(173, new byte[0], sendReliable: true, new RaiseEventOptions
			{
				Receivers = ReceiverGroup.Others
			});
			clip = Microphone.Start(DeviceName, loop: true, 100, (int)Frequency);
			ThreadId = UnityEngine.Random.Range(0, int.MaxValue);
			new Thread((ThreadStart)delegate
			{
				try
				{
					int threadId = ThreadId;
					Thread.CurrentThread.IsBackground = true;
					while (threadId == ThreadId && !Disconnected)
					{
						Thread.Sleep(300);
						SendMicData();
						if (threadId != ThreadId)
						{
							lastPos = 0;
							Microphone.End(DeviceName);
						}
					}
				}
				catch (Exception message)
				{
					MonoBehaviour.print(message);
				}
			}).Start();
		}
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		if (Users.ContainsKey(player.Id))
		{
			Users.Remove(player.Id);
			if (AdjustableList.Contains(player.Id))
			{
				AdjustableList.Remove(player.Id);
				RecompileSendList();
			}
		}
	}

	public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
	{
		PhotonPlayer photonPlayer = playerAndUpdatedProps[0] as PhotonPlayer;
		Hashtable hashtable = playerAndUpdatedProps[1] as Hashtable;
		if (hashtable.ContainsKey("name") && hashtable["name"] is string && Users.ContainsKey(photonPlayer.Id))
		{
			Users[photonPlayer.Id].Name = ((string)hashtable["name"]).NGUIToUnity();
		}
	}

	private void SendMicData()
	{
		if (AdjustableList.Count <= 0)
		{
			return;
		}
		int position = Microphone.GetPosition(DeviceName);
		if (position < lastPos)
		{
			lastPos = 0;
		}
		int num = position - lastPos;
		if (num > 0)
		{
			float[] data = new float[num];
			clip.GetData(data, lastPos);
			byte[] array = GzipCompress(data);
			if (array.Length < 12000)
			{
				PhotonNetwork.RaiseEvent(173, array, sendReliable: false, new RaiseEventOptions
				{
					TargetActors = SendList
				});
			}
		}
		lastPos = position;
	}

	public static byte[] GzipCompress(float[] data)
	{
		if (data == null)
		{
			return null;
		}
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (GZipOutputStream gZipOutputStream = new GZipOutputStream(memoryStream))
			{
				byte[] array = new byte[data.Length * 4];
				Buffer.BlockCopy(data, 0, array, 0, array.Length);
				gZipOutputStream.Write(array, 0, array.Length);
				gZipOutputStream.Finish();
				return memoryStream.ToArray();
			}
		}
	}

	public static float[] GzipDecompress(byte[] bytes)
	{
		if (bytes == null)
		{
			return null;
		}
		using (MemoryStream baseInputStream = new MemoryStream(bytes))
		{
			using (GZipInputStream source = new GZipInputStream(baseInputStream))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					byte[] buffer = new byte[4096];
					StreamUtils.Copy(source, memoryStream, buffer);
					byte[] array = memoryStream.ToArray();
					float[] array2 = new float[array.Length / 4];
					Buffer.BlockCopy(array, 0, array2, 0, array.Length);
					return array2;
				}
			}
		}
	}
}

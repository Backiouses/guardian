using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Guardian.AntiAbuse.Validators
{
	internal class NetworkChecker
	{
		public static List<object> PropertyWhitelist = new List<object>(new object[2]
		{
			byte.MaxValue,
			"sender"
		});

		private static List<object> RoomPropertyWhitelist = new List<object>(new object[7]
		{
			byte.MaxValue,
			(byte)254,
			(byte)253,
			(byte)250,
			(byte)249,
			(byte)248,
			"sender"
		});

		public static void Init()
		{
			FieldInfo[] fields = typeof(PhotonPlayerProperty).GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				PropertyWhitelist.Add((string)fieldInfo.GetValue(null));
			}
		}

		public static bool IsInstantiatePacketValid(Hashtable evData, PhotonPlayer sender)
		{
			if (evData == null || !evData.ContainsKey((byte)0) || !(evData[(byte)0] is string) || (evData.ContainsKey((byte)1) && !(evData[(byte)1] is Vector3)) || (evData.ContainsKey((byte)2) && !(evData[(byte)2] is Quaternion)) || (evData.ContainsKey((byte)3) && !(evData[(byte)3] is int)) || (evData.ContainsKey((byte)4) && !(evData[(byte)4] is int[])) || (evData.ContainsKey((byte)5) && !(evData[(byte)5] is object[])) || !evData.ContainsKey((byte)6) || !evData.ContainsKey((byte)7) || (evData.ContainsKey((byte)8) && !(evData[(byte)8] is short)))
			{
				GuardianClient.Logger.Error("E(202) Malformed instantiate from #" + ((sender == null) ? "?" : sender.Id.ToString()) + ".");
				if (sender != null && !FengGameManagerMKII.IgnoreList.Contains(sender.Id))
				{
					FengGameManagerMKII.IgnoreList.Add(sender.Id);
				}
				return false;
			}
			return true;
		}

		public static bool IsSerializeReadValid(Hashtable data, PhotonPlayer sender)
		{
			if (data == null || !data.ContainsKey((byte)0) || !(data[(byte)0] is int) || !data.ContainsKey((byte)1) || !(data[(byte)1] is object[]))
			{
				GuardianClient.Logger.Error("E(201/206) Malformed serialization from #" + ((sender == null) ? "?" : sender.Id.ToString()) + ".");
				if (sender != null && !FengGameManagerMKII.IgnoreList.Contains(sender.Id))
				{
					FengGameManagerMKII.IgnoreList.Add(sender.Id);
				}
				return false;
			}
			return true;
		}

		public static bool IsRPCValid(Hashtable rpcData, PhotonPlayer sender)
		{
			if (rpcData == null || !rpcData.ContainsKey((byte)0) || !(rpcData[(byte)0] is int) || (rpcData.ContainsKey((byte)1) && !(rpcData[(byte)1] is short)) || !rpcData.ContainsKey((byte)2) || !(rpcData[(byte)2] is int) || (rpcData.ContainsKey((byte)3) && (!(rpcData[(byte)3] is string) || rpcData.ContainsKey((byte)5))) || (rpcData.ContainsKey((byte)4) && !(rpcData[(byte)4] is object[])) || (rpcData.ContainsKey((byte)5) && (!(rpcData[(byte)5] is byte) || rpcData.ContainsKey((byte)3))))
			{
				GuardianClient.Logger.Error("E(200) Malformed RPC from #" + ((sender == null) ? "?" : sender.Id.ToString()) + ".");
				if (sender != null && !FengGameManagerMKII.IgnoreList.Contains(sender.Id))
				{
					FengGameManagerMKII.IgnoreList.Add(sender.Id);
				}
				return false;
			}
			return true;
		}

		public static bool IsPVDestroyValid(PhotonView[] views, PhotonPlayer sender)
		{
			if (views != null && views.Length != 0 && views[0].ownerId != sender.Id && !sender.isMasterClient)
			{
				GuardianClient.Logger.Error($"E(204) Object.Destroy from #{sender.Id}.");
				if (!FengGameManagerMKII.IgnoreList.Contains(sender.Id))
				{
					FengGameManagerMKII.IgnoreList.Add(sender.Id);
				}
				return false;
			}
			return true;
		}

		public static bool IsStateChangeValid(PhotonPlayer sender)
		{
			if (sender == null)
			{
				return true;
			}
			GuardianClient.Logger.Error($"E(228) State Change from #{sender.Id}.");
			if (sender != null && !FengGameManagerMKII.IgnoreList.Contains(sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(sender.Id);
			}
			return false;
		}

		public static void OnPlayerPropertyModification(object[] playerAndUpdatedProps)
		{
			PhotonPlayer obj = playerAndUpdatedProps[0] as PhotonPlayer;
			Hashtable properties = playerAndUpdatedProps[1] as Hashtable;
			if (!obj.isLocal || !properties.ContainsKey("sender") || !(properties["sender"] is PhotonPlayer photonPlayer) || photonPlayer.isLocal)
			{
				return;
			}
			properties.StripKeysWithNullValues();
			List<object> keys = properties.Keys.ToList();
			PropertyWhitelist.ForEach(delegate(object k)
			{
				keys.Remove(k);
			});
			if (keys.Count >= 1)
			{
				GuardianClient.Logger.Error("#" + ((photonPlayer == null) ? "?" : photonPlayer.Id.ToString()) + " applied foreign properties to you.");
				string text = string.Join(", ", keys.Select((object k) => $"{{{k}={properties[k]}}}").ToArray());
				GuardianClient.Logger.Error("Properties: " + text);
				Hashtable nullified = new Hashtable();
				keys.ForEach(delegate(object v)
				{
					nullified.Add(v, null);
				});
				PhotonNetwork.player.SetCustomProperties(nullified);
				if (photonPlayer != null && !FengGameManagerMKII.IgnoreList.Contains(photonPlayer.Id))
				{
					FengGameManagerMKII.IgnoreList.Add(photonPlayer.Id);
				}
			}
		}

		public static void OnRoomPropertyModification(Hashtable propertiesThatChanged)
		{
			if (!PhotonNetwork.isMasterClient || !propertiesThatChanged.ContainsKey("sender") || !(propertiesThatChanged["sender"] is PhotonPlayer photonPlayer) || photonPlayer.isLocal || photonPlayer.isMasterClient)
			{
				return;
			}
			propertiesThatChanged.StripKeysWithNullValues();
			List<object> keys = propertiesThatChanged.Keys.ToList();
			RoomPropertyWhitelist.ForEach(delegate(object k)
			{
				keys.Remove(k);
			});
			if (keys.Count > 0)
			{
				GuardianClient.Logger.Error($"#{photonPlayer.Id} applied foreign properties to the room.");
				string text = string.Join(", ", keys.Select((object k) => $"{{{k}={propertiesThatChanged[k]}}}").ToArray());
				GuardianClient.Logger.Error("Properties: " + text);
				Hashtable nullified = new Hashtable();
				keys.ForEach(delegate(object v)
				{
					nullified.Add(v, null);
				});
				PhotonNetwork.room.SetCustomProperties(nullified);
			}
			if (PhotonNetwork.room.expectedJoinability != PhotonNetwork.room.open)
			{
				PhotonNetwork.room.open = PhotonNetwork.room.expectedJoinability;
			}
			if (PhotonNetwork.room.expectedVisibility != PhotonNetwork.room.visible)
			{
				PhotonNetwork.room.visible = PhotonNetwork.room.expectedVisibility;
			}
			if (PhotonNetwork.room.expectedMaxPlayers != PhotonNetwork.room.maxPlayers)
			{
				PhotonNetwork.room.maxPlayers = PhotonNetwork.room.expectedMaxPlayers;
			}
		}
	}
}

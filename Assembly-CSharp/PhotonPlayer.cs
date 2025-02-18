using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonPlayer
{
	private string nameField = string.Empty;

	public readonly bool isLocal;

	public int Id = -1;

	public bool IsInactive;

	public bool Muted;

	public int Ping = -1;

	public bool IsAnarchyMod;

	public bool IsAnarchyExpMod;

	public bool IsCyanMod;

	public bool IsCyrusMod;

	public bool IsExpeditionMod;

	public bool IsFoxMod;

	public bool IsKNKMod;

	public bool IsNekoMod;

	public bool IsNekoModOwner;

	public bool IsNekoModUser;

	public bool IsNewRCMod;

	public bool IsNRCMod;

	public bool IsPBMod;

	public bool IsPhotonMod;

	public bool IsRC83Mod;

	public bool IsRRCMod;

	public bool IsTRAPMod;

	public bool IsUniverseMod;

	public bool IsUnknownMod;

	public string name
	{
		get
		{
			return nameField;
		}
		set
		{
			if (!isLocal)
			{
				Debug.LogError("Error: Cannot change the name of a remote player!");
			}
			else
			{
				nameField = value;
			}
		}
	}

	public bool isMasterClient => PhotonNetwork.networkingPeer.mMasterClient == this;

	public Hashtable customProperties { get; private set; }

	public Hashtable allProperties
	{
		get
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Merge(customProperties);
			hashtable[byte.MaxValue] = name;
			return hashtable;
		}
	}

	public string Username
	{
		get
		{
			if (customProperties.ContainsKey(PhotonPlayerProperty.Name) && customProperties[PhotonPlayerProperty.Name] is string result)
			{
				return result;
			}
			return string.Empty;
		}
	}

	public string Guild
	{
		get
		{
			if (customProperties.ContainsKey(PhotonPlayerProperty.Guild) && customProperties[PhotonPlayerProperty.Guild] is string result)
			{
				return result;
			}
			return string.Empty;
		}
	}

	public bool IsDead
	{
		get
		{
			if (customProperties.ContainsKey(PhotonPlayerProperty.IsDead))
			{
				object obj = customProperties[PhotonPlayerProperty.IsDead];
				if (obj is bool)
				{
					return (bool)obj;
				}
			}
			return false;
		}
	}

	public int Team
	{
		get
		{
			if (customProperties.ContainsKey(PhotonPlayerProperty.Team))
			{
				object obj = customProperties[PhotonPlayerProperty.Team];
				if (obj is int)
				{
					return (int)obj;
				}
			}
			return 0;
		}
	}

	public bool IsAhss => Team == 2;

	public bool IsTitan
	{
		get
		{
			if (customProperties.ContainsKey(PhotonPlayerProperty.IsTitan) && customProperties[PhotonPlayerProperty.IsTitan] is int num)
			{
				return num == 2;
			}
			return false;
		}
	}

	public int Kills
	{
		get
		{
			if (customProperties.ContainsKey(PhotonPlayerProperty.Kills))
			{
				object obj = customProperties[PhotonPlayerProperty.Kills];
				if (obj is int)
				{
					return (int)obj;
				}
			}
			return 0;
		}
	}

	public int Deaths
	{
		get
		{
			if (customProperties.ContainsKey(PhotonPlayerProperty.Deaths))
			{
				object obj = customProperties[PhotonPlayerProperty.Deaths];
				if (obj is int)
				{
					return (int)obj;
				}
			}
			return 0;
		}
	}

	public int MaxDamage
	{
		get
		{
			if (customProperties.ContainsKey(PhotonPlayerProperty.MaxDamage))
			{
				object obj = customProperties[PhotonPlayerProperty.MaxDamage];
				if (obj is int)
				{
					return (int)obj;
				}
			}
			return 0;
		}
	}

	public int TotalDamage
	{
		get
		{
			if (customProperties.ContainsKey(PhotonPlayerProperty.TotalDamage))
			{
				object obj = customProperties[PhotonPlayerProperty.TotalDamage];
				if (obj is int)
				{
					return (int)obj;
				}
			}
			return 0;
		}
	}

	public int SpeedStat
	{
		get
		{
			if (customProperties.ContainsKey(PhotonPlayerProperty.StatSpeed))
			{
				object obj = customProperties[PhotonPlayerProperty.StatSpeed];
				if (obj is int)
				{
					return (int)obj;
				}
			}
			return 0;
		}
	}

	public int BladeStat
	{
		get
		{
			if (customProperties.ContainsKey(PhotonPlayerProperty.StatBlade))
			{
				object obj = customProperties[PhotonPlayerProperty.StatBlade];
				if (obj is int)
				{
					return (int)obj;
				}
			}
			return 0;
		}
	}

	public int GasStat
	{
		get
		{
			if (customProperties.ContainsKey(PhotonPlayerProperty.StatGas))
			{
				object obj = customProperties[PhotonPlayerProperty.StatGas];
				if (obj is int)
				{
					return (int)obj;
				}
			}
			return 0;
		}
	}

	public int AccelStat
	{
		get
		{
			if (customProperties.ContainsKey(PhotonPlayerProperty.StatAccel))
			{
				object obj = customProperties[PhotonPlayerProperty.StatAccel];
				if (obj is int)
				{
					return (int)obj;
				}
			}
			return 0;
		}
	}

	public PhotonPlayer(bool isLocal, int actorID, string name)
	{
		customProperties = new Hashtable();
		this.isLocal = isLocal;
		Id = actorID;
		nameField = name;
	}

	protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
	{
		customProperties = new Hashtable();
		this.isLocal = isLocal;
		Id = actorID;
		InternalCacheProperties(properties);
	}

	public override bool Equals(object p)
	{
		if (p is PhotonPlayer photonPlayer)
		{
			return GetHashCode() == photonPlayer.GetHashCode();
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Id;
	}

	internal void InternalChangeLocalID(int newId)
	{
		if (!isLocal)
		{
			Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
		}
		else
		{
			Id = newId;
		}
	}

	internal void InternalCacheProperties(Hashtable properties)
	{
		if (properties != null && properties.Count != 0 && !customProperties.Equals(properties))
		{
			if (properties.ContainsKey(byte.MaxValue))
			{
				nameField = (string)properties[byte.MaxValue];
			}
			customProperties.MergeStringKeys(properties);
			customProperties.StripKeysWithNullValues();
		}
	}

	public void SetCustomProperties(Hashtable propertiesToSet, bool broadcast = true)
	{
		if (propertiesToSet != null)
		{
			customProperties.MergeStringKeys(propertiesToSet);
			customProperties.StripKeysWithNullValues();
			Hashtable actorProperties = propertiesToSet.StripToStringKeys();
			if (Id > 0 && !PhotonNetwork.offlineMode)
			{
				PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfActor(Id, actorProperties, broadcast, 0);
			}
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, this, propertiesToSet);
		}
	}

	public static PhotonPlayer Find(int id)
	{
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			PhotonPlayer photonPlayer = PhotonNetwork.playerList[i];
			if (photonPlayer.Id == id)
			{
				return photonPlayer;
			}
		}
		return null;
	}

	public PhotonPlayer GetNext()
	{
		return GetNextFor(Id);
	}

	public PhotonPlayer GetNextFor(PhotonPlayer currentPlayer)
	{
		if (currentPlayer == null)
		{
			return null;
		}
		return GetNextFor(currentPlayer.Id);
	}

	public PhotonPlayer GetNextFor(int currentPlayerId)
	{
		if (PhotonNetwork.networkingPeer == null || PhotonNetwork.networkingPeer.mActors == null || PhotonNetwork.networkingPeer.mActors.Count < 2)
		{
			return null;
		}
		Dictionary<int, PhotonPlayer> mActors = PhotonNetwork.networkingPeer.mActors;
		int num = int.MaxValue;
		int num2 = currentPlayerId;
		foreach (int key in mActors.Keys)
		{
			if (key < num2)
			{
				num2 = key;
			}
			else if (key > currentPlayerId && key < num)
			{
				num = key;
			}
		}
		if (num != int.MaxValue)
		{
			return mActors[num];
		}
		return mActors[num2];
	}

	public HERO GetHero()
	{
		foreach (HERO hero in FengGameManagerMKII.Instance.Heroes)
		{
			if (hero.photonView.ownerId == Id)
			{
				return hero;
			}
		}
		return null;
	}

	public TITAN GetTitan()
	{
		foreach (TITAN titan in FengGameManagerMKII.Instance.Titans)
		{
			if (titan.photonView.ownerId == Id)
			{
				return titan;
			}
		}
		return null;
	}

	public override string ToString()
	{
		if (string.IsNullOrEmpty(name))
		{
			return string.Format("#{0:00}{1}", Id, (!isMasterClient) ? string.Empty : "(master)");
		}
		return string.Format("'{0}'{1}", name, (!isMasterClient) ? string.Empty : "(master)");
	}

	public string ToStringFull()
	{
		return $"#{Id:00} '{name}' {customProperties.ToStringFull()}";
	}
}

using System;

public class RCActionHelper
{
	public enum helperClasses
	{
		primitive,
		variable,
		player,
		titan,
		region,
		convert
	}

	public enum mathTypes
	{
		add,
		subtract,
		multiply,
		divide,
		modulo,
		power
	}

	public enum variableTypes
	{
		typeInt,
		typeBool,
		typeString,
		typeFloat,
		typePlayer,
		typeTitan
	}

	public enum playerTypes
	{
		playerType,
		playerTeam,
		playerAlive,
		playerTitan,
		playerKills,
		playerDeaths,
		playerMaxDamage,
		playerTotalDamage,
		playerCustomInt,
		playerCustomBool,
		playerCustomString,
		playerCustomFloat,
		playerName,
		playerGuildName,
		playerPosX,
		playerPosY,
		playerPosZ,
		playerSpeed
	}

	public enum titanTypes
	{
		titanType,
		titanSize,
		titanHealth,
		positionX,
		positionY,
		positionZ
	}

	public enum other
	{
		regionX,
		regionY,
		regionZ
	}

	public int helperClass;

	public int helperType;

	private object parameters;

	private RCActionHelper nextHelper;

	public RCActionHelper(int sentClass, int sentType, object options)
	{
		helperClass = sentClass;
		helperType = sentType;
		parameters = options;
	}

	public void SetNextHelper(RCActionHelper sentHelper)
	{
		nextHelper = sentHelper;
	}

	public void CallException(string str)
	{
		InRoomChat.Instance.AddLine(str);
	}

	public int ReturnInt(object sentObject)
	{
		object obj = sentObject;
		if (parameters != null)
		{
			obj = parameters;
		}
		switch (helperClass)
		{
		case 0:
			return (int)obj;
		case 5:
			switch (helperType)
			{
			case 0:
				return (int)obj;
			case 1:
				return Convert.ToInt32((bool)obj);
			case 3:
				return Convert.ToInt32((float)obj);
			case 2:
			{
				_ = (string)obj;
				if (int.TryParse((string)obj, out var result))
				{
					return result;
				}
				return 0;
			}
			default:
				return (int)obj;
			}
		case 1:
		{
			RCActionHelper rCActionHelper2 = (RCActionHelper)obj;
			switch (helperType)
			{
			case 0:
				return (int)FengGameManagerMKII.IntVariables[rCActionHelper2.ReturnString(null)];
			case 1:
				return nextHelper.ReturnInt(FengGameManagerMKII.BoolVariables[rCActionHelper2.ReturnString(null)]);
			case 2:
				return nextHelper.ReturnInt(FengGameManagerMKII.StringVariables[rCActionHelper2.ReturnString(null)]);
			case 3:
				return nextHelper.ReturnInt(FengGameManagerMKII.FloatVariables[rCActionHelper2.ReturnString(null)]);
			case 4:
				return nextHelper.ReturnInt(FengGameManagerMKII.PlayerVariables[rCActionHelper2.ReturnString(null)]);
			case 5:
				return nextHelper.ReturnInt(FengGameManagerMKII.TitanVariables[rCActionHelper2.ReturnString(null)]);
			default:
				return 0;
			}
		}
		case 2:
		{
			PhotonPlayer photonPlayer = (PhotonPlayer)obj;
			if (photonPlayer != null)
			{
				switch (helperType)
				{
				case 0:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.Team];
				case 1:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.RCTeam];
				case 2:
					return nextHelper.ReturnInt(photonPlayer.customProperties[PhotonPlayerProperty.IsDead]);
				case 3:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.IsTitan];
				case 4:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.Kills];
				case 5:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.Deaths];
				case 6:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.MaxDamage];
				case 7:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.TotalDamage];
				case 8:
					return (int)photonPlayer.customProperties[PhotonPlayerProperty.CustomInt];
				case 9:
					return nextHelper.ReturnInt(photonPlayer.customProperties[PhotonPlayerProperty.CustomBool]);
				case 10:
					return nextHelper.ReturnInt(photonPlayer.customProperties[PhotonPlayerProperty.CustomString]);
				case 11:
					return nextHelper.ReturnInt(photonPlayer.customProperties[PhotonPlayerProperty.CustomFloat]);
				case 14:
				{
					int id4 = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id4))
					{
						HERO hERO4 = (HERO)FengGameManagerMKII.HeroHash[id4];
						return nextHelper.ReturnInt(hERO4.transform.position.x);
					}
					return 0;
				}
				case 15:
				{
					int id3 = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id3))
					{
						HERO hERO3 = (HERO)FengGameManagerMKII.HeroHash[id3];
						return nextHelper.ReturnInt(hERO3.transform.position.y);
					}
					return 0;
				}
				case 16:
				{
					int id2 = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id2))
					{
						HERO hERO2 = (HERO)FengGameManagerMKII.HeroHash[id2];
						return nextHelper.ReturnInt(hERO2.transform.position.z);
					}
					return 0;
				}
				case 12:
					return nextHelper.ReturnInt(photonPlayer.customProperties[PhotonPlayerProperty.Name]);
				case 13:
					return nextHelper.ReturnInt(photonPlayer.customProperties[PhotonPlayerProperty.Guild]);
				case 17:
				{
					int id = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id))
					{
						HERO hERO = (HERO)FengGameManagerMKII.HeroHash[id];
						return nextHelper.ReturnInt(hERO.rigidbody.velocity.magnitude);
					}
					return 0;
				}
				}
			}
			return 0;
		}
		case 3:
		{
			TITAN tITAN = (TITAN)obj;
			if (tITAN != null)
			{
				switch (helperType)
				{
				case 0:
					return (int)tITAN.abnormalType;
				case 1:
					return nextHelper.ReturnInt(tITAN.myLevel);
				case 2:
					return tITAN.currentHealth;
				case 3:
					return nextHelper.ReturnInt(tITAN.transform.position.x);
				case 4:
					return nextHelper.ReturnInt(tITAN.transform.position.y);
				case 5:
					return nextHelper.ReturnInt(tITAN.transform.position.z);
				}
			}
			return 0;
		}
		case 4:
		{
			RCActionHelper rCActionHelper = (RCActionHelper)obj;
			RCRegion rCRegion = (RCRegion)FengGameManagerMKII.RCRegions[rCActionHelper.ReturnString(null)];
			switch (helperType)
			{
			case 0:
				return nextHelper.ReturnInt(rCRegion.GetRandomX());
			case 1:
				return nextHelper.ReturnInt(rCRegion.GetRandomY());
			case 2:
				return nextHelper.ReturnInt(rCRegion.GetRandomZ());
			default:
				return 0;
			}
		}
		default:
			return 0;
		}
	}

	public bool ReturnBool(object sentObject)
	{
		object obj = sentObject;
		if (parameters != null)
		{
			obj = parameters;
		}
		switch (helperClass)
		{
		case 0:
			return (bool)obj;
		case 5:
			switch (helperType)
			{
			case 1:
				return (bool)obj;
			case 0:
				return Convert.ToBoolean((int)obj);
			case 2:
				return Convert.ToBoolean((string)obj);
			case 3:
				return Convert.ToBoolean((float)obj);
			default:
				return false;
			}
		case 1:
		{
			RCActionHelper rCActionHelper2 = (RCActionHelper)obj;
			switch (helperType)
			{
			case 1:
				return (bool)FengGameManagerMKII.BoolVariables[rCActionHelper2.ReturnString(null)];
			case 0:
				return nextHelper.ReturnBool(FengGameManagerMKII.IntVariables[rCActionHelper2.ReturnString(null)]);
			case 2:
				return nextHelper.ReturnBool(FengGameManagerMKII.StringVariables[rCActionHelper2.ReturnString(null)]);
			case 3:
				return nextHelper.ReturnBool(FengGameManagerMKII.FloatVariables[rCActionHelper2.ReturnString(null)]);
			case 4:
				return nextHelper.ReturnBool(FengGameManagerMKII.PlayerVariables[rCActionHelper2.ReturnString(null)]);
			case 5:
				return nextHelper.ReturnBool(FengGameManagerMKII.TitanVariables[rCActionHelper2.ReturnString(null)]);
			default:
				return false;
			}
		}
		case 2:
		{
			PhotonPlayer photonPlayer = (PhotonPlayer)obj;
			if (photonPlayer != null)
			{
				switch (helperType)
				{
				case 0:
					return nextHelper.ReturnBool(photonPlayer.customProperties[PhotonPlayerProperty.Team]);
				case 1:
					return nextHelper.ReturnBool(photonPlayer.customProperties[PhotonPlayerProperty.RCTeam]);
				case 2:
					return !(bool)photonPlayer.customProperties[PhotonPlayerProperty.IsDead];
				case 3:
					return nextHelper.ReturnBool(photonPlayer.customProperties[PhotonPlayerProperty.IsTitan]);
				case 4:
					return nextHelper.ReturnBool(photonPlayer.customProperties[PhotonPlayerProperty.Kills]);
				case 5:
					return nextHelper.ReturnBool(photonPlayer.customProperties[PhotonPlayerProperty.Deaths]);
				case 6:
					return nextHelper.ReturnBool(photonPlayer.customProperties[PhotonPlayerProperty.MaxDamage]);
				case 7:
					return nextHelper.ReturnBool(photonPlayer.customProperties[PhotonPlayerProperty.TotalDamage]);
				case 8:
					return nextHelper.ReturnBool(photonPlayer.customProperties[PhotonPlayerProperty.CustomInt]);
				case 9:
					return (bool)photonPlayer.customProperties[PhotonPlayerProperty.CustomBool];
				case 10:
					return nextHelper.ReturnBool(photonPlayer.customProperties[PhotonPlayerProperty.CustomString]);
				case 11:
					return nextHelper.ReturnBool(photonPlayer.customProperties[PhotonPlayerProperty.CustomFloat]);
				case 14:
				{
					int id4 = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id4))
					{
						HERO hERO4 = (HERO)FengGameManagerMKII.HeroHash[id4];
						return nextHelper.ReturnBool(hERO4.transform.position.x);
					}
					return false;
				}
				case 15:
				{
					int id3 = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id3))
					{
						HERO hERO3 = (HERO)FengGameManagerMKII.HeroHash[id3];
						return nextHelper.ReturnBool(hERO3.transform.position.y);
					}
					return false;
				}
				case 16:
				{
					int id2 = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id2))
					{
						HERO hERO2 = (HERO)FengGameManagerMKII.HeroHash[id2];
						return nextHelper.ReturnBool(hERO2.transform.position.z);
					}
					return false;
				}
				case 12:
					return nextHelper.ReturnBool(photonPlayer.customProperties[PhotonPlayerProperty.Name]);
				case 13:
					return nextHelper.ReturnBool(photonPlayer.customProperties[PhotonPlayerProperty.Guild]);
				case 17:
				{
					int id = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id))
					{
						HERO hERO = (HERO)FengGameManagerMKII.HeroHash[id];
						return nextHelper.ReturnBool(hERO.rigidbody.velocity.magnitude);
					}
					return false;
				}
				}
			}
			return false;
		}
		case 3:
		{
			TITAN tITAN = (TITAN)obj;
			if (tITAN != null)
			{
				switch (helperType)
				{
				case 0:
					return nextHelper.ReturnBool(tITAN.abnormalType);
				case 1:
					return nextHelper.ReturnBool(tITAN.myLevel);
				case 2:
					return nextHelper.ReturnBool(tITAN.currentHealth);
				case 3:
					return nextHelper.ReturnBool(tITAN.transform.position.x);
				case 4:
					return nextHelper.ReturnBool(tITAN.transform.position.y);
				case 5:
					return nextHelper.ReturnBool(tITAN.transform.position.z);
				}
			}
			return false;
		}
		case 4:
		{
			RCActionHelper rCActionHelper = (RCActionHelper)obj;
			RCRegion rCRegion = (RCRegion)FengGameManagerMKII.RCRegions[rCActionHelper.ReturnString(null)];
			switch (helperType)
			{
			case 0:
				return nextHelper.ReturnBool(rCRegion.GetRandomX());
			case 1:
				return nextHelper.ReturnBool(rCRegion.GetRandomY());
			case 2:
				return nextHelper.ReturnBool(rCRegion.GetRandomZ());
			default:
				return false;
			}
		}
		default:
			return false;
		}
	}

	public string ReturnString(object sentObject)
	{
		object obj = sentObject;
		if (parameters != null)
		{
			obj = parameters;
		}
		switch (helperClass)
		{
		case 0:
			return (string)obj;
		case 5:
			switch (helperType)
			{
			case 0:
				return ((int)obj).ToString();
			case 1:
				return ((bool)obj).ToString();
			case 3:
				return ((float)obj).ToString();
			case 2:
				return (string)obj;
			default:
				return string.Empty;
			}
		case 1:
		{
			RCActionHelper rCActionHelper2 = (RCActionHelper)obj;
			switch (helperType)
			{
			case 0:
				return nextHelper.ReturnString(FengGameManagerMKII.IntVariables[rCActionHelper2.ReturnString(null)]);
			case 1:
				return nextHelper.ReturnString(FengGameManagerMKII.BoolVariables[rCActionHelper2.ReturnString(null)]);
			case 2:
				return (string)FengGameManagerMKII.StringVariables[rCActionHelper2.ReturnString(null)];
			case 3:
				return nextHelper.ReturnString(FengGameManagerMKII.FloatVariables[rCActionHelper2.ReturnString(null)]);
			case 4:
				return nextHelper.ReturnString(FengGameManagerMKII.PlayerVariables[rCActionHelper2.ReturnString(null)]);
			case 5:
				return nextHelper.ReturnString(FengGameManagerMKII.TitanVariables[rCActionHelper2.ReturnString(null)]);
			default:
				return string.Empty;
			}
		}
		case 2:
		{
			PhotonPlayer photonPlayer = (PhotonPlayer)obj;
			if (photonPlayer != null)
			{
				switch (helperType)
				{
				case 0:
					return nextHelper.ReturnString(photonPlayer.customProperties[PhotonPlayerProperty.Team]);
				case 1:
					return nextHelper.ReturnString(photonPlayer.customProperties[PhotonPlayerProperty.RCTeam]);
				case 2:
					return nextHelper.ReturnString(photonPlayer.customProperties[PhotonPlayerProperty.IsDead]);
				case 3:
					return nextHelper.ReturnString(photonPlayer.customProperties[PhotonPlayerProperty.IsTitan]);
				case 4:
					return nextHelper.ReturnString(photonPlayer.customProperties[PhotonPlayerProperty.Kills]);
				case 5:
					return nextHelper.ReturnString(photonPlayer.customProperties[PhotonPlayerProperty.Deaths]);
				case 6:
					return nextHelper.ReturnString(photonPlayer.customProperties[PhotonPlayerProperty.MaxDamage]);
				case 7:
					return nextHelper.ReturnString(photonPlayer.customProperties[PhotonPlayerProperty.TotalDamage]);
				case 8:
					return nextHelper.ReturnString(photonPlayer.customProperties[PhotonPlayerProperty.CustomInt]);
				case 9:
					return nextHelper.ReturnString(photonPlayer.customProperties[PhotonPlayerProperty.CustomBool]);
				case 10:
					return (string)photonPlayer.customProperties[PhotonPlayerProperty.CustomString];
				case 11:
					return nextHelper.ReturnString(photonPlayer.customProperties[PhotonPlayerProperty.CustomFloat]);
				case 14:
				{
					int id4 = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id4))
					{
						HERO hERO4 = (HERO)FengGameManagerMKII.HeroHash[id4];
						return nextHelper.ReturnString(hERO4.transform.position.x);
					}
					return string.Empty;
				}
				case 15:
				{
					int id3 = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id3))
					{
						HERO hERO3 = (HERO)FengGameManagerMKII.HeroHash[id3];
						return nextHelper.ReturnString(hERO3.transform.position.y);
					}
					return string.Empty;
				}
				case 16:
				{
					int id2 = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id2))
					{
						HERO hERO2 = (HERO)FengGameManagerMKII.HeroHash[id2];
						return nextHelper.ReturnString(hERO2.transform.position.z);
					}
					return string.Empty;
				}
				case 12:
					return (string)photonPlayer.customProperties[PhotonPlayerProperty.Name];
				case 13:
					return (string)photonPlayer.customProperties[PhotonPlayerProperty.Guild];
				case 17:
				{
					int id = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id))
					{
						HERO hERO = (HERO)FengGameManagerMKII.HeroHash[id];
						return nextHelper.ReturnString(hERO.rigidbody.velocity.magnitude);
					}
					return string.Empty;
				}
				}
			}
			return string.Empty;
		}
		case 3:
		{
			TITAN tITAN = (TITAN)obj;
			if (tITAN != null)
			{
				switch (helperType)
				{
				case 0:
					return nextHelper.ReturnString(tITAN.abnormalType);
				case 1:
					return nextHelper.ReturnString(tITAN.myLevel);
				case 2:
					return nextHelper.ReturnString(tITAN.currentHealth);
				case 3:
					return nextHelper.ReturnString(tITAN.transform.position.x);
				case 4:
					return nextHelper.ReturnString(tITAN.transform.position.y);
				case 5:
					return nextHelper.ReturnString(tITAN.transform.position.z);
				}
			}
			return string.Empty;
		}
		case 4:
		{
			RCActionHelper rCActionHelper = (RCActionHelper)obj;
			RCRegion rCRegion = (RCRegion)FengGameManagerMKII.RCRegions[rCActionHelper.ReturnString(null)];
			switch (helperType)
			{
			case 0:
				return nextHelper.ReturnString(rCRegion.GetRandomX());
			case 1:
				return nextHelper.ReturnString(rCRegion.GetRandomY());
			case 2:
				return nextHelper.ReturnString(rCRegion.GetRandomZ());
			default:
				return string.Empty;
			}
		}
		default:
			return string.Empty;
		}
	}

	public float ReturnFloat(object sentObject)
	{
		object obj = sentObject;
		if (parameters != null)
		{
			obj = parameters;
		}
		switch (helperClass)
		{
		case 0:
			return (float)obj;
		case 5:
			switch (helperType)
			{
			case 0:
				return Convert.ToSingle((int)obj);
			case 1:
				return Convert.ToSingle((bool)obj);
			case 3:
				return (float)obj;
			case 2:
			{
				_ = (string)obj;
				if (float.TryParse((string)obj, out var result))
				{
					return result;
				}
				return 0f;
			}
			default:
				return (float)obj;
			}
		case 1:
		{
			RCActionHelper rCActionHelper2 = (RCActionHelper)obj;
			switch (helperType)
			{
			case 0:
				return nextHelper.ReturnFloat(FengGameManagerMKII.IntVariables[rCActionHelper2.ReturnString(null)]);
			case 1:
				return nextHelper.ReturnFloat(FengGameManagerMKII.BoolVariables[rCActionHelper2.ReturnString(null)]);
			case 2:
				return nextHelper.ReturnFloat(FengGameManagerMKII.StringVariables[rCActionHelper2.ReturnString(null)]);
			case 3:
				return (float)FengGameManagerMKII.FloatVariables[rCActionHelper2.ReturnString(null)];
			case 4:
				return nextHelper.ReturnFloat(FengGameManagerMKII.PlayerVariables[rCActionHelper2.ReturnString(null)]);
			case 5:
				return nextHelper.ReturnFloat(FengGameManagerMKII.TitanVariables[rCActionHelper2.ReturnString(null)]);
			default:
				return 0f;
			}
		}
		case 2:
		{
			PhotonPlayer photonPlayer = (PhotonPlayer)obj;
			if (photonPlayer != null)
			{
				switch (helperType)
				{
				case 0:
					return nextHelper.ReturnFloat(photonPlayer.customProperties[PhotonPlayerProperty.Team]);
				case 1:
					return nextHelper.ReturnFloat(photonPlayer.customProperties[PhotonPlayerProperty.RCTeam]);
				case 2:
					return nextHelper.ReturnFloat(photonPlayer.customProperties[PhotonPlayerProperty.IsDead]);
				case 3:
					return nextHelper.ReturnFloat(photonPlayer.customProperties[PhotonPlayerProperty.IsTitan]);
				case 4:
					return nextHelper.ReturnFloat(photonPlayer.customProperties[PhotonPlayerProperty.Kills]);
				case 5:
					return nextHelper.ReturnFloat(photonPlayer.customProperties[PhotonPlayerProperty.Deaths]);
				case 6:
					return nextHelper.ReturnFloat(photonPlayer.customProperties[PhotonPlayerProperty.MaxDamage]);
				case 7:
					return nextHelper.ReturnFloat(photonPlayer.customProperties[PhotonPlayerProperty.TotalDamage]);
				case 8:
					return nextHelper.ReturnFloat(photonPlayer.customProperties[PhotonPlayerProperty.CustomInt]);
				case 9:
					return nextHelper.ReturnFloat(photonPlayer.customProperties[PhotonPlayerProperty.CustomBool]);
				case 10:
					return nextHelper.ReturnFloat(photonPlayer.customProperties[PhotonPlayerProperty.CustomString]);
				case 11:
					return (float)photonPlayer.customProperties[PhotonPlayerProperty.CustomFloat];
				case 14:
				{
					int id4 = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id4))
					{
						return ((HERO)FengGameManagerMKII.HeroHash[id4]).transform.position.x;
					}
					return 0f;
				}
				case 15:
				{
					int id3 = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id3))
					{
						return ((HERO)FengGameManagerMKII.HeroHash[id3]).transform.position.y;
					}
					return 0f;
				}
				case 16:
				{
					int id2 = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id2))
					{
						return ((HERO)FengGameManagerMKII.HeroHash[id2]).transform.position.z;
					}
					return 0f;
				}
				case 12:
					return nextHelper.ReturnFloat(photonPlayer.customProperties[PhotonPlayerProperty.Name]);
				case 13:
					return nextHelper.ReturnFloat(photonPlayer.customProperties[PhotonPlayerProperty.Guild]);
				case 17:
				{
					int id = photonPlayer.Id;
					if (FengGameManagerMKII.HeroHash.ContainsKey(id))
					{
						return ((HERO)FengGameManagerMKII.HeroHash[id]).rigidbody.velocity.magnitude;
					}
					return 0f;
				}
				}
			}
			return 0f;
		}
		case 3:
		{
			TITAN tITAN = (TITAN)obj;
			if (tITAN != null)
			{
				switch (helperType)
				{
				case 0:
					return nextHelper.ReturnFloat(tITAN.abnormalType);
				case 1:
					return tITAN.myLevel;
				case 2:
					return nextHelper.ReturnFloat(tITAN.currentHealth);
				case 3:
					return tITAN.transform.position.x;
				case 4:
					return tITAN.transform.position.y;
				case 5:
					return tITAN.transform.position.z;
				}
			}
			return 0f;
		}
		case 4:
		{
			RCActionHelper rCActionHelper = (RCActionHelper)obj;
			RCRegion rCRegion = (RCRegion)FengGameManagerMKII.RCRegions[rCActionHelper.ReturnString(null)];
			switch (helperType)
			{
			case 0:
				return rCRegion.GetRandomX();
			case 1:
				return rCRegion.GetRandomY();
			case 2:
				return rCRegion.GetRandomZ();
			default:
				return 0f;
			}
		}
		default:
			return 0f;
		}
	}

	public PhotonPlayer ReturnPlayer(object objParameter)
	{
		object obj = objParameter;
		if (parameters != null)
		{
			obj = parameters;
		}
		switch (helperClass)
		{
		case 1:
		{
			RCActionHelper rCActionHelper = (RCActionHelper)obj;
			return (PhotonPlayer)FengGameManagerMKII.PlayerVariables[rCActionHelper.ReturnString(null)];
		}
		case 2:
			return (PhotonPlayer)obj;
		default:
			return (PhotonPlayer)obj;
		}
	}

	public TITAN ReturnTitan(object objParameter)
	{
		object obj = objParameter;
		if (parameters != null)
		{
			obj = parameters;
		}
		switch (helperClass)
		{
		case 1:
		{
			RCActionHelper rCActionHelper = (RCActionHelper)obj;
			return (TITAN)FengGameManagerMKII.TitanVariables[rCActionHelper.ReturnString(null)];
		}
		case 3:
			return (TITAN)obj;
		default:
			return (TITAN)obj;
		}
	}
}

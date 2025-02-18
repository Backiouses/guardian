using System;
using System.Collections.Generic;
using UnityEngine;

public class PunTeams : MonoBehaviour
{
	public enum Team : byte
	{
		none,
		red,
		blue
	}

	public const string TeamPlayerProp = "team";

	public static Dictionary<Team, List<PhotonPlayer>> PlayersPerTeam;

	public void Start()
	{
		PlayersPerTeam = new Dictionary<Team, List<PhotonPlayer>>();
		foreach (object value in Enum.GetValues(typeof(Team)))
		{
			PlayersPerTeam[(Team)(byte)value] = new List<PhotonPlayer>();
		}
	}

	public void OnJoinedRoom()
	{
		UpdateTeams();
	}

	public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
	{
		UpdateTeams();
	}

	public void UpdateTeams()
	{
		foreach (object value in Enum.GetValues(typeof(Team)))
		{
			PlayersPerTeam[(Team)(byte)value].Clear();
		}
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			PhotonPlayer photonPlayer = PhotonNetwork.playerList[i];
			Team team = photonPlayer.GetTeam();
			PlayersPerTeam[team].Add(photonPlayer);
		}
	}
}

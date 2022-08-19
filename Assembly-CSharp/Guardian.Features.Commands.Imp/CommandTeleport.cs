using System;
using Guardian.Utilities;
using Photon;
using UnityEngine;

namespace Guardian.Features.Commands.Impl.MasterClient
{
	internal class CommandTeleport : Command
	{
		public CommandTeleport()
			: base("teleport", new string[1] { "tp" }, "<id/all/x y z> [id/x y z]", masterClient: true)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (FengGameManagerMKII.Level.Mode == GameMode.Racing)
			{
				irc.AddLine("Teleport can NOT be used while in Racing.".AsColor("FF0000"));
			}
			else if (args.Length > 3)
			{
				if (!float.TryParse(args[1], out var result) || !float.TryParse(args[2], out var result2) || !float.TryParse(args[3], out var result3))
				{
					return;
				}
				if (args[0].Equals("all", StringComparison.OrdinalIgnoreCase))
				{
					PhotonPlayer[] playerList = PhotonNetwork.playerList;
					foreach (PhotonPlayer photonPlayer in playerList)
					{
						Photon.MonoBehaviour monoBehaviour = (photonPlayer.IsTitan ? ((Photon.MonoBehaviour)photonPlayer.GetTitan()) : ((Photon.MonoBehaviour)photonPlayer.GetHero()));
						if (!(monoBehaviour == null))
						{
							monoBehaviour.photonView.RPC("moveToRPC", photonPlayer, result, result2, result3);
						}
					}
					GameHelper.Broadcast($"Teleported everyone to {result:F3} / {result2:F3} / {result3:F3}");
				}
				else
				{
					if (!int.TryParse(args[0], out var result4))
					{
						return;
					}
					PhotonPlayer photonPlayer2 = PhotonPlayer.Find(result4);
					if (photonPlayer2 != null)
					{
						Photon.MonoBehaviour monoBehaviour2 = (photonPlayer2.IsTitan ? ((Photon.MonoBehaviour)photonPlayer2.GetTitan()) : ((Photon.MonoBehaviour)photonPlayer2.GetHero()));
						if (!(monoBehaviour2 == null))
						{
							monoBehaviour2.photonView.RPC("moveToRPC", photonPlayer2, result, result2, result3);
							FengGameManagerMKII.Instance.photonView.RPC("Chat", photonPlayer2, $"Teleported you to {result:F3} {result2:F3} {result3:F3}", string.Empty);
						}
					}
				}
			}
			else if (args.Length > 2)
			{
				if (float.TryParse(args[0], out var result5) && float.TryParse(args[1], out var result6) && float.TryParse(args[2], out var result7))
				{
					Photon.MonoBehaviour monoBehaviour3 = (PhotonNetwork.player.IsTitan ? ((Photon.MonoBehaviour)PhotonNetwork.player.GetTitan()) : ((Photon.MonoBehaviour)PhotonNetwork.player.GetHero()));
					if (!(monoBehaviour3 == null))
					{
						monoBehaviour3.transform.position = new Vector3(result5, result6, result7);
						irc.AddLine($"Teleported you to {result5:F3} {result6:F3} {result7:F3}");
					}
				}
			}
			else if (args.Length > 1)
			{
				if (!int.TryParse(args[1], out var result8))
				{
					return;
				}
				PhotonPlayer photonPlayer3 = PhotonPlayer.Find(result8);
				if (photonPlayer3 == null)
				{
					return;
				}
				Photon.MonoBehaviour monoBehaviour4 = (photonPlayer3.IsTitan ? ((Photon.MonoBehaviour)photonPlayer3.GetTitan()) : ((Photon.MonoBehaviour)photonPlayer3.GetHero()));
				if (monoBehaviour4 == null)
				{
					return;
				}
				if (args[0].Equals("all", StringComparison.OrdinalIgnoreCase))
				{
					PhotonPlayer[] playerList = PhotonNetwork.playerList;
					foreach (PhotonPlayer photonPlayer4 in playerList)
					{
						Photon.MonoBehaviour monoBehaviour5 = (photonPlayer4.IsTitan ? ((Photon.MonoBehaviour)photonPlayer4.GetTitan()) : ((Photon.MonoBehaviour)photonPlayer4.GetHero()));
						if (!(monoBehaviour5 == null))
						{
							monoBehaviour5.photonView.RPC("moveToRPC", photonPlayer4, monoBehaviour4.transform.position.x, monoBehaviour4.transform.position.y, monoBehaviour4.transform.position.z);
						}
					}
					GameHelper.Broadcast($"Teleported everyone to #{result8}");
				}
				else
				{
					if (!int.TryParse(args[0], out var result9))
					{
						return;
					}
					PhotonPlayer photonPlayer5 = PhotonPlayer.Find(result9);
					if (photonPlayer5 != null)
					{
						Photon.MonoBehaviour monoBehaviour6 = (photonPlayer5.IsTitan ? ((Photon.MonoBehaviour)photonPlayer5.GetTitan()) : ((Photon.MonoBehaviour)photonPlayer5.GetHero()));
						if (!(monoBehaviour6 == null))
						{
							monoBehaviour6.photonView.RPC("moveToRPC", photonPlayer5, monoBehaviour4.transform.position.x, monoBehaviour4.transform.position.y, monoBehaviour4.transform.position.z);
							FengGameManagerMKII.Instance.photonView.RPC("Chat", photonPlayer5, $"Teleported you to #{result8}", string.Empty);
						}
					}
				}
			}
			else
			{
				if (args.Length == 0)
				{
					return;
				}
				Photon.MonoBehaviour monoBehaviour7 = (PhotonNetwork.player.IsTitan ? ((Photon.MonoBehaviour)PhotonNetwork.player.GetTitan()) : ((Photon.MonoBehaviour)PhotonNetwork.player.GetHero()));
				if (monoBehaviour7 == null)
				{
					return;
				}
				if (args[0].Equals("all", StringComparison.OrdinalIgnoreCase))
				{
					PhotonPlayer[] playerList = PhotonNetwork.playerList;
					foreach (PhotonPlayer photonPlayer6 in playerList)
					{
						Photon.MonoBehaviour monoBehaviour8 = (photonPlayer6.IsTitan ? ((Photon.MonoBehaviour)photonPlayer6.GetTitan()) : ((Photon.MonoBehaviour)photonPlayer6.GetHero()));
						if (!(monoBehaviour8 == null))
						{
							monoBehaviour8.photonView.RPC("moveToRPC", photonPlayer6, monoBehaviour7.transform.position.x, monoBehaviour7.transform.position.y, monoBehaviour7.transform.position.z);
						}
					}
					GameHelper.Broadcast("Teleported everyone to MasterClient!");
				}
				else
				{
					if (!int.TryParse(args[0], out var result10))
					{
						return;
					}
					PhotonPlayer photonPlayer7 = PhotonPlayer.Find(result10);
					if (photonPlayer7 != null)
					{
						Photon.MonoBehaviour monoBehaviour9 = (photonPlayer7.IsTitan ? ((Photon.MonoBehaviour)photonPlayer7.GetTitan()) : ((Photon.MonoBehaviour)photonPlayer7.GetHero()));
						if (!(monoBehaviour9 == null))
						{
							monoBehaviour7.transform.position = monoBehaviour9.transform.position;
							irc.AddLine($"Teleported you to #{result10}");
						}
					}
				}
			}
		}
	}
}

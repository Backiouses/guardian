using System;
using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl.RC.MasterClient
{
	internal class CommandRevive : Command
	{
		public CommandRevive()
			: base("revive", new string[4] { "heal", "respawn", "rev", "res" }, "[all/id]", masterClient: true)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length != 0)
			{
				int result;
				if (args[0].Equals("all", StringComparison.OrdinalIgnoreCase))
				{
					PhotonPlayer[] playerList = PhotonNetwork.playerList;
					foreach (PhotonPlayer photonPlayer in playerList)
					{
						if (photonPlayer.IsDead && !photonPlayer.IsTitan)
						{
							FengGameManagerMKII.Instance.photonView.RPC("respawnHeroInNewRound", photonPlayer);
						}
					}
					GameHelper.Broadcast("All players have been revived.");
				}
				else if (int.TryParse(args[0], out result))
				{
					PhotonPlayer photonPlayer2 = PhotonPlayer.Find(result);
					if (photonPlayer2 != null && photonPlayer2.IsDead && !photonPlayer2.IsTitan)
					{
						FengGameManagerMKII.Instance.photonView.RPC("respawnHeroInNewRound", photonPlayer2);
						irc.AddLine($"Revived #{result}.");
					}
				}
			}
			else if (PhotonNetwork.player.IsDead && !PhotonNetwork.player.IsTitan)
			{
				FengGameManagerMKII.Instance.photonView.RPC("respawnHeroInNewRound", PhotonNetwork.player);
				irc.AddLine("Revived self.");
			}
		}
	}
}

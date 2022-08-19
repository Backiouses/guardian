using System;
using ExitGames.Client.Photon;
using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl.RC
{
	internal class CommandResetKD : Command
	{
		public CommandResetKD()
			: base("resetkd", new string[0], "[all/id]", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			Hashtable propertiesToSet = new Hashtable
			{
				{
					PhotonPlayerProperty.Kills,
					0
				},
				{
					PhotonPlayerProperty.Deaths,
					0
				},
				{
					PhotonPlayerProperty.MaxDamage,
					0
				},
				{
					PhotonPlayerProperty.TotalDamage,
					0
				}
			};
			if (args.Length != 0)
			{
				int result;
				if (!PhotonNetwork.isMasterClient)
				{
					irc.AddLine("Command requires master client!".AsColor("FF0000"));
				}
				else if (args[0].Equals("all", StringComparison.OrdinalIgnoreCase))
				{
					PhotonPlayer[] playerList = PhotonNetwork.playerList;
					for (int i = 0; i < playerList.Length; i++)
					{
						playerList[i].SetCustomProperties(propertiesToSet);
					}
					GameHelper.Broadcast("All stats have been reset.");
				}
				else if (int.TryParse(args[0], out result))
				{
					PhotonPlayer photonPlayer = PhotonPlayer.Find(result);
					if (photonPlayer != null)
					{
						photonPlayer.SetCustomProperties(propertiesToSet);
						irc.AddLine($"You reset #{result}'s stats.".AsColor("FFCC00"));
					}
				}
			}
			else
			{
				PhotonNetwork.player.SetCustomProperties(propertiesToSet);
				irc.AddLine("Your stats have been reset.".AsColor("FFCC00"));
			}
		}
	}
}

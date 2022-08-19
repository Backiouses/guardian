using System;
using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl.RC.MasterClient
{
	internal class CommandRoom : Command
	{
		public CommandRoom()
			: base("room", new string[0], "<time/max/open/visible/pttl/rttl> <value>", masterClient: true)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length < 2)
			{
				return;
			}
			switch (args[0].ToLower())
			{
			case "time":
			{
				if (int.TryParse(args[1], out var result2))
				{
					FengGameManagerMKII.Instance.AddTime(result2);
					GameHelper.Broadcast($"Added {result2}s to the clock!");
				}
				break;
			}
			case "max":
			{
				if (int.TryParse(args[1], out var result4))
				{
					PhotonNetwork.room.expectedMaxPlayers = result4;
					PhotonNetwork.room.maxPlayers = PhotonNetwork.room.expectedMaxPlayers;
					GameHelper.Broadcast($"Max players is now {result4}!");
				}
				break;
			}
			case "open":
				PhotonNetwork.room.expectedJoinability = args[1].Equals("true", StringComparison.OrdinalIgnoreCase);
				PhotonNetwork.room.open = PhotonNetwork.room.expectedJoinability;
				GameHelper.Broadcast("Room is " + (PhotonNetwork.room.open ? "now" : "no longer") + " allowing joins!");
				break;
			case "visible":
				PhotonNetwork.room.expectedVisibility = args[1].Equals("true", StringComparison.OrdinalIgnoreCase);
				PhotonNetwork.room.visible = PhotonNetwork.room.expectedVisibility;
				GameHelper.Broadcast("Room is " + (PhotonNetwork.room.visible ? "now" : "no longer") + " being shown in the lobby!");
				break;
			case "pttl":
			{
				if (int.TryParse(args[1], out var result3))
				{
					if (PhotonNetwork.player.Id == 1)
					{
						PhotonNetwork.room.playerTtl = result3;
						GameHelper.Broadcast($"Player TTL is now {result3}ms!");
					}
					else
					{
						irc.AddLine("You must be the room creator to execute this!".AsColor("FF0000"));
					}
				}
				break;
			}
			case "rttl":
			{
				if (int.TryParse(args[1], out var result))
				{
					if (PhotonNetwork.player.Id == 1)
					{
						PhotonNetwork.room.emptyRoomTtl = result;
						GameHelper.Broadcast($"Room TTL is now {result}ms!");
					}
					else
					{
						irc.AddLine("You must be the room creator to execute this!".AsColor("FF0000"));
					}
				}
				break;
			}
			}
		}
	}
}

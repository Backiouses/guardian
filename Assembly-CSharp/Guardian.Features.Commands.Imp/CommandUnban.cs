using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl.RC
{
	internal class CommandUnban : Command
	{
		public CommandUnban()
			: base("unban", new string[1] { "pardon" }, "<id>", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length < 1 || !int.TryParse(args[0], out var result))
			{
				return;
			}
			if (FengGameManagerMKII.OnPrivateServer)
			{
				FengGameManagerMKII.ServerRequestUnban(result.ToString());
			}
			else if (PhotonNetwork.isMasterClient)
			{
				if (FengGameManagerMKII.BanHash.ContainsKey(result))
				{
					GameHelper.Broadcast((GExtensions.AsString(FengGameManagerMKII.BanHash[result]) + " has been unbanned.").AsColor("FFCC00"));
					FengGameManagerMKII.BanHash.Remove(result);
				}
			}
			else
			{
				irc.AddLine("Command requires master client.".AsColor("FF0000"));
			}
		}
	}
}

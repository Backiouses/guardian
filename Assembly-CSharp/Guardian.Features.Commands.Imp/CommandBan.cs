using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl.RC
{
	internal class CommandBan : Command
	{
		public CommandBan()
			: base("ban", new string[0], "<id> [reason]", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length < 1 || !int.TryParse(args[0], out var result))
			{
				return;
			}
			PhotonPlayer photonPlayer = PhotonPlayer.Find(result);
			if (photonPlayer == null || photonPlayer.isLocal || photonPlayer.isMasterClient)
			{
				return;
			}
			if (!PhotonNetwork.isMasterClient && !FengGameManagerMKII.OnPrivateServer)
			{
				string text = GExtensions.AsString(PhotonNetwork.player.customProperties[PhotonPlayerProperty.Name]).NGUIToUnity();
				if (text.Length == 0)
				{
					text = GExtensions.AsString(PhotonNetwork.player.customProperties[PhotonPlayerProperty.Name]);
				}
				FengGameManagerMKII.Instance.photonView.RPC("Chat", PhotonTargets.All, "/kick #" + result, text);
			}
			else
			{
				string text2 = ((args.Length > 1) ? string.Join(" ", args.CopyOfRange(1, args.Length)) : "Banned.");
				FengGameManagerMKII.Instance.KickPlayer(photonPlayer, ban: true, text2);
				if (!FengGameManagerMKII.OnPrivateServer)
				{
					GameHelper.Broadcast(GExtensions.AsString(photonPlayer.customProperties[PhotonPlayerProperty.Name]).NGUIToUnity() + " has been banned!");
					GameHelper.Broadcast("Reason: \"" + text2 + "\"");
				}
			}
		}
	}
}

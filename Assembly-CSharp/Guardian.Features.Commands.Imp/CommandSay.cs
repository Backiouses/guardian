namespace Guardian.Features.Commands.Impl
{
	internal class CommandSay : Command
	{
		public CommandSay()
			: base("say", new string[0], "<message>", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length >= 1)
			{
				string text = GExtensions.AsString(PhotonNetwork.player.customProperties[PhotonPlayerProperty.Name]).NGUIToUnity();
				if (text.StripNGUI().Length < 1)
				{
					text = GExtensions.AsString(PhotonNetwork.player.customProperties[PhotonPlayerProperty.Name]);
				}
				FengGameManagerMKII.Instance.photonView.RPC("Chat", PhotonTargets.All, InRoomChat.FormatMessage(string.Join(" ", args), text));
			}
		}
	}
}

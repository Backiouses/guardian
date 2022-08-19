namespace Guardian.Features.Commands.Impl
{
	internal class CommandMute : Command
	{
		public CommandMute()
			: base("mute", new string[0], "<id>", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length >= 1 && int.TryParse(args[0], out var result))
			{
				PhotonPlayer photonPlayer = PhotonPlayer.Find(result);
				if (photonPlayer != null && !photonPlayer.Muted)
				{
					photonPlayer.Muted = true;
					irc.AddLine($"Ignoring chat messages from #{result}.");
				}
			}
		}
	}
}

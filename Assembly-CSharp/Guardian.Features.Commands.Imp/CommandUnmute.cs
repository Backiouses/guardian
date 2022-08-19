namespace Guardian.Features.Commands.Impl
{
	internal class CommandUnmute : Command
	{
		public CommandUnmute()
			: base("unmute", new string[0], "<id>", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length >= 1 && int.TryParse(args[0], out var result))
			{
				PhotonPlayer photonPlayer = PhotonPlayer.Find(result);
				if (photonPlayer != null && photonPlayer.Muted)
				{
					photonPlayer.Muted = false;
					irc.AddLine($"No longer ignoring chat messages from #{result}.");
				}
			}
		}
	}
}

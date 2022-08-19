namespace Guardian.Features.Commands.Impl.RC
{
	internal class CommandPM : Command
	{
		public CommandPM()
			: base("pm", new string[4] { "w", "whisper", "tell", "msg" }, "<id> <message>", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length >= 2 && int.TryParse(args[0], out var result))
			{
				PhotonPlayer photonPlayer = PhotonPlayer.Find(result);
				if (photonPlayer != null)
				{
					string text = InRoomChat.FormatMessage(string.Join(" ", args.CopyOfRange(1, args.Length)), string.Empty)[0] as string;
					FengGameManagerMKII.Instance.photonView.RPC("Chat", photonPlayer, text, "PM => You".AsColor("FFCC00"));
					irc.AddLine($"You => #{photonPlayer.Id}".AsColor("FFCC00") + ": " + text);
				}
			}
		}
	}
}

namespace Guardian.Features.Commands.Impl
{
	internal class CommandIgnore : Command
	{
		public CommandIgnore()
			: base("ignore", new string[0], "<id>", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length >= 1 && int.TryParse(args[0], out var result) && !FengGameManagerMKII.IgnoreList.Contains(result))
			{
				FengGameManagerMKII.IgnoreList.Add(result);
				irc.AddLine($"Ignoring events from #{result}.".AsColor("FFCC00"));
			}
		}
	}
}

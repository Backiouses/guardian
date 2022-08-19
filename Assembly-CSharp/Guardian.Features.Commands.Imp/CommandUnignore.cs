namespace Guardian.Features.Commands.Impl
{
	internal class CommandUnignore : Command
	{
		public CommandUnignore()
			: base("unignore", new string[1] { "unig" }, "<id>", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length >= 1 && int.TryParse(args[0], out var result) && FengGameManagerMKII.IgnoreList.Contains(result))
			{
				FengGameManagerMKII.IgnoreList.Remove(result);
				irc.AddLine($"No longer ignoring events from #{result}.".AsColor("FFCC00"));
			}
		}
	}
}

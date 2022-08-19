namespace Guardian.Features.Commands.Impl.RC
{
	internal class CommandIgnoreList : Command
	{
		public CommandIgnoreList()
			: base("ignorelist", new string[1] { "ignored" }, string.Empty, masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			irc.AddLine("List of ignored players:".AsColor("FFCC00"));
			foreach (int ignore in FengGameManagerMKII.IgnoreList)
			{
				irc.AddLine(ignore.ToString());
			}
		}
	}
}

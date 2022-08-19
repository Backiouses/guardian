namespace Guardian.Features.Commands.Impl.RC.MasterClient
{
	internal class CommandBanlist : Command
	{
		public CommandBanlist()
			: base("banlist", new string[0], string.Empty, masterClient: true)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			irc.AddLine("List of banned players:".AsColor("FFCC00"));
			foreach (int key in FengGameManagerMKII.BanHash.Keys)
			{
				irc.AddLine($"#{key} ({GExtensions.AsString(FengGameManagerMKII.BanHash[key]).NGUIToUnity()})");
			}
		}
	}
}

using Guardian.AntiAbuse.Validators;

namespace Guardian.Features.Commands.Impl
{
	internal class CommandReloadConfig : Command
	{
		public CommandReloadConfig()
			: base("reloadconfig", new string[1] { "rlcfg" }, string.Empty, masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			irc.AddLine("Reloading configuration files...");
			GuardianClient.Properties.LoadFromFile();
			irc.AddLine("Configuration reloaded.");
			irc.AddLine("Reloading skin host whitelist...");
			SkinChecker.Init();
			irc.AddLine("Skin host whitelist reloaded.");
		}
	}
}

using Guardian.Features.Gamemodes;
using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl.MasterClient
{
	internal class CommandGamemode : Command
	{
		public CommandGamemode()
			: base("gamemode", new string[2] { "gm", "mode" }, "<mode>", masterClient: true)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length != 0)
			{
				Gamemode currentMode = GuardianClient.Gamemodes.CurrentMode;
				Gamemode gamemode = GuardianClient.Gamemodes.Find(args[0]);
				if (gamemode != null)
				{
					GameHelper.Broadcast("Gamemode Switch (" + currentMode.Name + " -> " + gamemode.Name + ")!");
					gamemode.OnReset();
					GuardianClient.Gamemodes.CurrentMode = gamemode;
					currentMode.CleanUp();
				}
				return;
			}
			irc.AddLine("Available Gamemodes:".AsColor("AAFF00"));
			foreach (Gamemode element in GuardianClient.Gamemodes.Elements)
			{
				irc.AddLine("> ".AsColor("00FF00").AsBold() + element.Name);
			}
		}
	}
}

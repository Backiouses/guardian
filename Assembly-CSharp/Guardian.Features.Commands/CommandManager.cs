using Guardian.Features.Commands.Impl;
using Guardian.Features.Commands.Impl.Debug;
using Guardian.Features.Commands.Impl.MasterClient;
using Guardian.Features.Commands.Impl.RC;
using Guardian.Features.Commands.Impl.RC.MasterClient;

namespace Guardian.Features.Commands
{
	internal class CommandManager : FeatureManager<Command>
	{
		public override void Load()
		{
			Add(new CommandHelp());
			Add(new CommandClear());
			Add(new CommandIgnore());
			Add(new CommandMute());
			Add(new CommandRageQuit());
			Add(new CommandRejoin());
			Add(new CommandReloadConfig());
			Add(new CommandSay());
			Add(new CommandScreenshot());
			Add(new CommandSetGuild());
			Add(new CommandSetLighting());
			Add(new CommandSetName());
			Add(new CommandTranslate());
			Add(new CommandUnignore());
			Add(new CommandUnmute());
			Add(new CommandWhois());
			Add(new CommandDifficulty());
			Add(new CommandGamemode());
			Add(new CommandGuestBeGone());
			Add(new CommandKill());
			Add(new CommandScatterTitans());
			Add(new CommandSetMap());
			Add(new CommandSetTitans());
			Add(new CommandTeleport());
			Add(new CommandLogProperties());
			Add(new CommandAso());
			Add(new CommandBanlist());
			Add(new CommandPause());
			Add(new CommandRestart());
			Add(new CommandRevive());
			Add(new CommandRoom());
			Add(new CommandUnpause());
			Add(new CommandBan());
			Add(new CommandIgnoreList());
			Add(new CommandKick());
			Add(new CommandPM());
			Add(new CommandResetKD());
			Add(new CommandRules());
			Add(new CommandSpectate());
			Add(new CommandTeam());
			Add(new CommandUnban());
			GuardianClient.Logger.Debug($"Registered {Elements.Count} commands.");
		}

		public void HandleCommand(InRoomChat irc)
		{
			string[] array = irc.inputLine.Trim().Substring(1).Split(' ');
			Command command = Find(array[0]);
			if (command != null)
			{
				if (!command.MasterClient || PhotonNetwork.isMasterClient)
				{
					command.Execute(irc, (array.Length > 1) ? array.CopyOfRange(1, array.Length) : new string[0]);
				}
				else
				{
					irc.AddLine("Command requires MasterClient!".AsColor("FF0000"));
				}
			}
			else if (array[0].Length > 0)
			{
				irc.AddLine(("Command '" + array[0] + "' not found.").AsColor("FF0000"));
			}
		}
	}
}

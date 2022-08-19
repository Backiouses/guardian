using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl
{
	internal class CommandHelp : Command
	{
		private int CommandsPerPage = 7;

		public CommandHelp()
			: base("help", new string[2] { "?", "commands" }, "[page/command]", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			int num = 0;
			int num2 = MathHelper.Ceil((float)GuardianClient.Commands.Elements.Count / (float)CommandsPerPage);
			if (args.Length != 0)
			{
				Command command = GuardianClient.Commands.Find(args[0]);
				if (command != null)
				{
					irc.AddLine(("Help for command '" + command.Name + "':").AsColor("AAFF00").AsBold());
					irc.AddLine("Usage: /" + command.Name + " " + command.Usage);
					irc.AddLine("Aliases: [" + string.Join(", ", command.Aliases) + "]");
					return;
				}
				if (int.TryParse(args[0], out var result))
				{
					num = MathHelper.Clamp(result, 1, num2) - 1;
				}
			}
			irc.AddLine("For general help regarding Guardian, visit".AsColor("FFFF00"));
			irc.AddLine("\thttps://winnpixie.github.io/guardian/".AsColor("0099FF") + "!".AsColor("FFFF00"));
			irc.AddLine($"Commands (Page {num + 1}/{num2})".AsColor("AAFF00").AsBold());
			irc.AddLine("<arg> = Required, [arg] = Optional".AsColor("AAAAAA").AsBold());
			for (int i = 0; i < CommandsPerPage; i++)
			{
				int num3 = i + num * CommandsPerPage;
				if (num3 < GuardianClient.Commands.Elements.Count)
				{
					Command command2 = GuardianClient.Commands.Elements[num3];
					string text = "> ".AsColor("00FF00").AsBold() + "/" + command2.Name + " " + command2.Usage;
					if (command2.MasterClient)
					{
						text += " [MC]".AsColor("FF0000").AsBold();
					}
					if (command2.GetType().Namespace.EndsWith("Debug"))
					{
						text += " [DBG]".AsColor("AAAAAA").AsBold();
					}
					irc.AddLine(text);
					continue;
				}
				break;
			}
		}
	}
}

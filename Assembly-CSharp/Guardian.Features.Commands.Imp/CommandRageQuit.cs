using UnityEngine;

namespace Guardian.Features.Commands.Impl
{
	internal class CommandRageQuit : Command
	{
		public CommandRageQuit()
			: base("ragequit", new string[3] { "rq", "quit", "leave" }, string.Empty, masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			Application.Quit();
		}
	}
}

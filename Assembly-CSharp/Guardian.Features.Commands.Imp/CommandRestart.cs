using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl.RC.MasterClient
{
	internal class CommandRestart : Command
	{
		public CommandRestart()
			: base("restart", new string[1] { "r" }, string.Empty, masterClient: true)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			FengGameManagerMKII.Instance.RestartRC();
			GameHelper.Broadcast("The round has been restarted!");
		}
	}
}

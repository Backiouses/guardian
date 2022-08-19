namespace Guardian.Features.Commands
{
	public abstract class Command : Feature
	{
		public string Usage;

		public bool MasterClient;

		public Command(string name, string[] aliases, string usage, bool masterClient)
			: base(name, aliases)
		{
			Usage = usage;
			MasterClient = masterClient;
		}

		public abstract void Execute(InRoomChat irc, string[] args);
	}
}

using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl.MasterClient
{
	internal class CommandDifficulty : Command
	{
		public CommandDifficulty()
			: base("difficulty", new string[0], "<training/normal/hard/abnormal>", masterClient: true)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length >= 1)
			{
				int num;
				switch (args[0].ToLower())
				{
				case "training":
					num = -1;
					break;
				case "normal":
					num = 0;
					break;
				case "hard":
					num = 1;
					break;
				case "abnormal":
					num = 2;
					break;
				default:
					num = -2;
					break;
				}
				int num2 = num;
				if (num2 >= -1)
				{
					FengGameManagerMKII.Instance.difficulty = num2;
					IN_GAME_MAIN_CAMERA.Difficulty = num2;
					GameHelper.Broadcast("Room difficulty is now " + args[0].ToUpper() + "!");
					GameHelper.Broadcast("This change will be effective on the next wave OR game restart.");
				}
			}
		}
	}
}

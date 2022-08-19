using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl.MasterClient
{
	internal class CommandSetTitans : Command
	{
		public CommandSetTitans()
			: base("settitans", new string[0], "<normal/aberrant/jumper/crawler/punk>", masterClient: true)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length < 1)
			{
				return;
			}
			TitanClass? titanClass;
			switch (args[0].ToLower())
			{
			case "normal":
				titanClass = TitanClass.Normal;
				break;
			case "aberrant":
				titanClass = TitanClass.Aberrant;
				break;
			case "jumper":
				titanClass = TitanClass.Jumper;
				break;
			case "crawler":
				titanClass = TitanClass.Crawler;
				break;
			case "punk":
				titanClass = TitanClass.Punk;
				break;
			default:
				titanClass = null;
				break;
			}
			TitanClass? titanClass2 = titanClass;
			if (!titanClass2.HasValue)
			{
				return;
			}
			foreach (TITAN titan in FengGameManagerMKII.Instance.Titans)
			{
				if (titan.photonView.isMine && !titanClass2.Equals(titan.abnormalType))
				{
					titan.setAbnormalType2(titanClass2 ?? titan.abnormalType, titanClass2 == TitanClass.Crawler);
				}
			}
			GameHelper.Broadcast($"All non-player titans are now of type {titanClass2.Value}!");
		}
	}
}

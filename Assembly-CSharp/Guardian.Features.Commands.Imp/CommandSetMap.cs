using ExitGames.Client.Photon;
using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl.MasterClient
{
	internal class CommandSetMap : Command
	{
		public CommandSetMap()
			: base("setmap", new string[1] { "map" }, "<name>", masterClient: true)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length != 0)
			{
				LevelInfo info = LevelInfo.GetInfo(string.Join(" ", args));
				if (info != null)
				{
					PhotonNetwork.room.SetCustomProperties(new Hashtable { { "Map", info.Name } });
					FengGameManagerMKII.Instance.RestartGame();
					GameHelper.Broadcast("The map in play is now " + info.Name + "!");
				}
			}
			else
			{
				irc.AddLine("Available Maps:".AsColor("AAFF00"));
				LevelInfo[] levels = LevelInfo.Levels;
				foreach (LevelInfo levelInfo in levels)
				{
					irc.AddLine("> ".AsColor("00FF00").AsBold() + levelInfo.Name);
				}
			}
		}
	}
}

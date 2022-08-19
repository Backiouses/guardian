using ExitGames.Client.Photon;

namespace Guardian.Features.Commands.Impl
{
	internal class CommandSetGuild : Command
	{
		public CommandSetGuild()
			: base("setguild", new string[1] { "guild" }, "[guild]", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			string text = ((args.Length != 0) ? string.Join(" ", args) : string.Empty);
			LoginFengKAI.Player.Guild = text;
			PhotonNetwork.player.SetCustomProperties(new Hashtable { 
			{
				PhotonPlayerProperty.Guild,
				text
			} });
			HERO hero = PhotonNetwork.player.GetHero();
			if (!(hero == null))
			{
				FengGameManagerMKII.Instance.photonView.RPC("labelRPC", PhotonTargets.All, hero.photonView.viewID);
			}
		}
	}
}

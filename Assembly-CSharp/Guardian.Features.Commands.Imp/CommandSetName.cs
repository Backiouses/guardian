using ExitGames.Client.Photon;

namespace Guardian.Features.Commands.Impl
{
	internal class CommandSetName : Command
	{
		public CommandSetName()
			: base("setname", new string[1] { "name" }, "[name]", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length >= 1)
			{
				string text = string.Join(" ", args);
				LoginFengKAI.Player.Name = text;
				FengGameManagerMKII.NameField = text;
				PhotonNetwork.player.SetCustomProperties(new Hashtable { 
				{
					PhotonPlayerProperty.Name,
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
}

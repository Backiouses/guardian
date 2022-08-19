namespace Guardian.Features.Commands.Impl.MasterClient
{
	internal class CommandKill : Command
	{
		public CommandKill()
			: base("kill", new string[0], "<id>", masterClient: true)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length < 1 || !int.TryParse(args[0], out var result))
			{
				return;
			}
			PhotonPlayer photonPlayer = PhotonPlayer.Find(result);
			if (photonPlayer == null)
			{
				return;
			}
			if (photonPlayer.IsTitan)
			{
				TITAN titan = photonPlayer.GetTitan();
				if (!(titan == null) && !titan.hasDie)
				{
					titan.photonView.RPC("titanGetHit", photonPlayer, titan.photonView.viewID, (RCSettings.MinimumDamage > 0) ? RCSettings.MinimumDamage : 10);
				}
				return;
			}
			HERO hero = photonPlayer.GetHero();
			if (!(hero == null) && !hero.HasDied())
			{
				hero.photonView.RPC("netDie", PhotonTargets.All, hero.transform.position, false, -1, "[FF0000]Server", true);
			}
		}
	}
}

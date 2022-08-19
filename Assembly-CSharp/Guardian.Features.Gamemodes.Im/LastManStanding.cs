using ExitGames.Client.Photon;
using Guardian.Features.Properties;
using Guardian.Utilities;

namespace Guardian.Features.Gamemodes.Impl
{
	internal class LastManStanding : Gamemode
	{
		private Property<int> KillInterval = new Property<int>("Gamemodes_LastManStanding:KillInterval", new string[0], 30);

		private long NextKillTime;

		private long LastPollTime;

		public LastManStanding()
			: base("LastManStanding", new string[1] { "lms" })
		{
			GuardianClient.Properties.Add(KillInterval);
		}

		public override void OnReset()
		{
			NextKillTime = GameHelper.CurrentTimeMillis() + KillInterval.Value * 1000;
			GameHelper.Broadcast($"Last Man Standing! Whoever has the least number of kills after every {KillInterval.Value} second period will DIE!");
			Hashtable propertiesToSet = new Hashtable
			{
				{
					PhotonPlayerProperty.Kills,
					0
				},
				{
					PhotonPlayerProperty.Deaths,
					0
				},
				{
					PhotonPlayerProperty.MaxDamage,
					0
				},
				{
					PhotonPlayerProperty.TotalDamage,
					0
				}
			};
			PhotonPlayer[] playerList = PhotonNetwork.playerList;
			for (int i = 0; i < playerList.Length; i++)
			{
				playerList[i].SetCustomProperties(propertiesToSet);
			}
		}

		public override void OnUpdate()
		{
			if (GameHelper.CurrentTimeMillis() - LastPollTime < 1000)
			{
				return;
			}
			LastPollTime = GameHelper.CurrentTimeMillis();
			if (GameHelper.CurrentTimeMillis() >= NextKillTime)
			{
				NextKillTime = GameHelper.CurrentTimeMillis() + KillInterval.Value * 1000;
				if (FengGameManagerMKII.Instance.Heroes.Count > 1)
				{
					int num = 0;
					int num2 = int.MinValue;
					HERO hERO = null;
					int num3 = int.MaxValue;
					HERO hERO2 = null;
					PhotonPlayer[] playerList = PhotonNetwork.playerList;
					foreach (PhotonPlayer photonPlayer in playerList)
					{
						HERO hero = photonPlayer.GetHero();
						if (!(hero == null))
						{
							num++;
							int num4 = GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.Kills]);
							if (num4 < num3)
							{
								num3 = num4;
								hERO2 = hero;
							}
							if (num4 > num2)
							{
								num2 = num4;
								hERO = hero;
							}
						}
					}
					if (hERO2 != null && num > 1)
					{
						PhotonNetwork.Instantiate("FX/Thunder", hERO2.transform.position, hERO2.transform.rotation, 0);
						hERO2.MarkDead();
						hERO2.photonView.RPC("netDie2", hERO2.photonView.owner, -1, "Lowest Kill Count");
						GameHelper.Broadcast((GExtensions.AsString(hERO2.photonView.owner.customProperties[PhotonPlayerProperty.Name]).NGUIToUnity().AsColor("FFFFFF") + " didn't make it!").AsColor("FF0000"));
					}
					if (num < 3 && hERO != null)
					{
						GameHelper.Broadcast((GExtensions.AsString(hERO.photonView.owner.customProperties[PhotonPlayerProperty.Name]).NGUIToUnity().AsColor("FFFFFF") + " wins!").AsColor("AAFF00"));
						FengGameManagerMKII.Instance.FinishGame();
						return;
					}
					Hashtable propertiesToSet = new Hashtable
					{
						{
							PhotonPlayerProperty.Kills,
							0
						},
						{
							PhotonPlayerProperty.Deaths,
							0
						},
						{
							PhotonPlayerProperty.MaxDamage,
							0
						},
						{
							PhotonPlayerProperty.TotalDamage,
							0
						}
					};
					playerList = PhotonNetwork.playerList;
					for (int i = 0; i < playerList.Length; i++)
					{
						playerList[i].SetCustomProperties(propertiesToSet);
					}
				}
				GameHelper.Broadcast($"A new {KillInterval.Value} second period has begun!");
			}
			else if (NextKillTime - GameHelper.CurrentTimeMillis() > 5000)
			{
				return;
			}
			int num5 = MathHelper.Floor((float)(NextKillTime - GameHelper.CurrentTimeMillis()) / 1000f) + 1;
			GameHelper.Broadcast($"{num5}...".AsColor("FF0000"));
		}

		public override void OnPlayerJoin(PhotonPlayer player)
		{
			FengGameManagerMKII.Instance.photonView.RPC("Chat", player, $"Last Man Standing! Whoever has the least number of kills after every {KillInterval.Value} second period will DIE!", string.Empty);
		}
	}
}

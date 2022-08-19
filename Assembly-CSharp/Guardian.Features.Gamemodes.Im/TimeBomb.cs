using System.Collections.Generic;
using ExitGames.Client.Photon;
using Guardian.Features.Properties;
using Guardian.Utilities;

namespace Guardian.Features.Gamemodes.Impl
{
	internal class TimeBomb : Gamemode
	{
		private Dictionary<int, int> LifeTimes;

		private Property<int> StartTime = new Property<int>("Gamemodes_TimeBomb:StartTime", new string[0], 90);

		private Property<float> ScoreMultiplier = new Property<float>("Gamemodes_TimeBomb:ScoreMultiplier", new string[0], 1f);

		private long LastPollTime;

		public TimeBomb()
			: base("TimeBomb", new string[1] { "tb" })
		{
			GuardianClient.Properties.Add(StartTime);
			GuardianClient.Properties.Add(ScoreMultiplier);
		}

		public override void CleanUp()
		{
			LifeTimes.Clear();
			if (PhotonNetwork.inRoom)
			{
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
		}

		public override void OnReset()
		{
			LifeTimes = new Dictionary<int, int>();
			LastPollTime = GameHelper.CurrentTimeMillis() + 1000;
			PhotonPlayer[] playerList = PhotonNetwork.playerList;
			foreach (PhotonPlayer photonPlayer in playerList)
			{
				LifeTimes.Add(photonPlayer.Id, StartTime.Value);
			}
			GameHelper.Broadcast("Tick-Tock! Time-Bomb mode is enabled, kill titans to stay alive!");
			GameHelper.Broadcast($"Everyone has been given a {StartTime.Value} second starting time.");
		}

		public override void OnUpdate()
		{
			if (GameHelper.CurrentTimeMillis() - LastPollTime < 1000)
			{
				return;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>(LifeTimes);
			LastPollTime = GameHelper.CurrentTimeMillis();
			foreach (KeyValuePair<int, int> lifeTime in LifeTimes)
			{
				PhotonPlayer photonPlayer = PhotonPlayer.Find(lifeTime.Key);
				if (photonPlayer == null)
				{
					continue;
				}
				HERO hero = photonPlayer.GetHero();
				if (!(hero == null))
				{
					int num = lifeTime.Value;
					if (lifeTime.Value >= 0)
					{
						num--;
						photonPlayer.SetCustomProperties(new Hashtable { 
						{
							PhotonPlayerProperty.TotalDamage,
							num
						} });
					}
					if (num <= 0)
					{
						PhotonNetwork.Instantiate("FX/Thunder", hero.transform.position, hero.transform.rotation, 0);
						hero.MarkDead();
						hero.photonView.RPC("netDie2", photonPlayer, -1, "[FF4444]Time's Up!");
						GameHelper.Broadcast((GExtensions.AsString(photonPlayer.customProperties[PhotonPlayerProperty.Name]).NGUIToUnity().AsColor("FFFFFF") + " ran out of time!").AsColor("FF0000"));
						LifeTimes[hero.photonView.owner.Id] = StartTime.Value;
					}
					else if (num == 15)
					{
						FengGameManagerMKII.Instance.photonView.RPC("Chat", photonPlayer, "15 seconds left...".AsColor("FF0000"), string.Empty);
					}
					else if (num < 6)
					{
						FengGameManagerMKII.Instance.photonView.RPC("Chat", photonPlayer, $"{num}...".AsColor("FF0000"), string.Empty);
					}
					dictionary[lifeTime.Key] = num;
				}
			}
			LifeTimes = dictionary;
		}

		public override void OnPlayerJoin(PhotonPlayer player)
		{
			FengGameManagerMKII.Instance.photonView.RPC("Chat", player, "Tick-Tock! Time-Bomb mode is enabled, kill titans to stay alive!", string.Empty);
			LifeTimes.Add(player.Id, StartTime.Value);
		}

		public override void OnPlayerLeave(PhotonPlayer player)
		{
			LifeTimes.Remove(player.Id);
		}

		public override void OnPlayerKilled(HERO hero, int killerId, bool wasKilledByTitan)
		{
			LifeTimes[hero.photonView.owner.Id] = StartTime.Value;
		}

		public override void OnTitanKilled(TITAN titan, PhotonPlayer killer, int damage)
		{
			int num = MathHelper.Floor((float)damage / 100f * ScoreMultiplier.Value);
			LifeTimes[killer.Id] += num;
			FengGameManagerMKII.Instance.photonView.RPC("Chat", killer, ("+" + num + "second(s)!").AsColor("00FF00"), string.Empty);
		}
	}
}

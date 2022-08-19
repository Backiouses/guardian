using Guardian.Features.Properties;
using Guardian.Utilities;
using UnityEngine;

namespace Guardian.Features.Gamemodes.Impl
{
	internal class CageFight : Gamemode
	{
		private Property<int> GroundLevel = new Property<int>("Gamemodes_CageFight:GroundLevel", new string[0], 0);

		private Property<int> LeftMinX = new Property<int>("Gamemodes_CageFight:MinX-Left", new string[0], -400);

		private Property<int> LeftMaxX = new Property<int>("Gamemodes_CageFight:MaxX-Left", new string[0], -50);

		private Property<int> LeftMinZ = new Property<int>("Gamemodes_CageFight:MinZ-Left", new string[0], -400);

		private Property<int> LeftMaxZ = new Property<int>("Gamemodes_CageFight:MaxZ-Left", new string[0], 400);

		private Property<int> RightMinX = new Property<int>("Gamemodes_CageFight:MinX-Right", new string[0], 50);

		private Property<int> RightMaxX = new Property<int>("Gamemodes_CageFight:MaxX-Right", new string[0], 400);

		private Property<int> RightMinZ = new Property<int>("Gamemodes_CageFight:MinZ-Right", new string[0], -400);

		private Property<int> RightMaxZ = new Property<int>("Gamemodes_CageFight:MaxZ-Right", new string[0], 400);

		private long RoundStartTime;

		private PhotonPlayer PlayerOne;

		private PhotonPlayer PlayerTwo;

		private bool GameOver;

		public CageFight()
			: base("CageFight", new string[3] { "cf", "standoff", "sf" })
		{
			GuardianClient.Properties.Add(GroundLevel);
			GuardianClient.Properties.Add(LeftMinX);
			GuardianClient.Properties.Add(LeftMaxX);
			GuardianClient.Properties.Add(LeftMinZ);
			GuardianClient.Properties.Add(LeftMaxZ);
			GuardianClient.Properties.Add(RightMinX);
			GuardianClient.Properties.Add(RightMaxX);
			GuardianClient.Properties.Add(RightMinZ);
			GuardianClient.Properties.Add(RightMaxZ);
		}

		public override void OnReset()
		{
			GameOver = false;
			RoundStartTime = -1L;
			PlayerOne = null;
			PlayerTwo = null;
			if (!FengGameManagerMKII.Level.Name.StartsWith("Custom"))
			{
				InRoomChat.Instance.AddLine("Cage Fight requires either Custom or Custom (No PT) to work!".AsColor("FF0000"));
				return;
			}
			if (PhotonNetwork.room.playerCount < 2)
			{
				InRoomChat.Instance.AddLine("Cage Fight requires two players to work!".AsColor("FF0000"));
				return;
			}
			RoundStartTime = GameHelper.CurrentTimeMillis();
			GameHelper.Broadcast("Cage Fight! Each kill puts another titan in the opponents ring, whoever dies first loses!");
			GameHelper.Broadcast("Starting in 5 seconds...");
		}

		public override void OnUpdate()
		{
			if (RoundStartTime == -1 || GameHelper.CurrentTimeMillis() - RoundStartTime < 5000)
			{
				return;
			}
			RoundStartTime = -1L;
			PlayerOne = PhotonNetwork.playerList[MathHelper.RandomInt(0, PhotonNetwork.playerList.Length)];
			do
			{
				PlayerTwo = PhotonNetwork.playerList[MathHelper.RandomInt(0, PhotonNetwork.playerList.Length)];
			}
			while (PlayerOne == PlayerTwo);
			HERO hero = PlayerOne.GetHero();
			HERO hero2 = PlayerTwo.GetHero();
			if (hero != null && hero2 != null)
			{
				float num = (float)(LeftMinX.Value + LeftMaxX.Value) / 2f;
				float num2 = (float)(LeftMinZ.Value + LeftMaxZ.Value) / 2f;
				hero.photonView.RPC("moveToRPC", PlayerOne, num, (float)GroundLevel.Value, num2);
				float num3 = (float)(RightMinX.Value + RightMaxX.Value) / 2f;
				float num4 = (float)(RightMinZ.Value + RightMaxZ.Value) / 2f;
				hero2.photonView.RPC("moveToRPC", PlayerTwo, num3, (float)GroundLevel.Value, num4);
			}
			else
			{
				InRoomChat.Instance.AddLine("One or more players are not spawned in! Trying again in 5 seconds...");
				RoundStartTime = GameHelper.CurrentTimeMillis();
			}
			foreach (TITAN titan in FengGameManagerMKII.Instance.Titans)
			{
				PhotonNetwork.Destroy(titan.gameObject);
			}
			SpawnTitan(0);
			SpawnTitan(1);
		}

		public override void OnPlayerLeave(PhotonPlayer player)
		{
			if (!GameOver && (player == PlayerOne || player == PlayerTwo))
			{
				GameOver = true;
				GameHelper.Broadcast((GExtensions.AsString(player.customProperties[PhotonPlayerProperty.Name]).NGUIToUnity().AsColor("FFFFFF") + " forfeit!").AsColor("FF0000"));
				FengGameManagerMKII.Instance.FinishGame();
			}
		}

		public override void OnPlayerKilled(HERO hero, int killerId, bool wasKilledByTitan)
		{
			if (!GameOver && (hero.photonView.owner == PlayerOne || hero.photonView.owner == PlayerTwo))
			{
				GameOver = true;
				if (hero.photonView.owner == PlayerOne)
				{
					GameHelper.Broadcast((GExtensions.AsString(PlayerTwo.customProperties[PhotonPlayerProperty.Name]).NGUIToUnity().AsColor("FFFFFF") + " wins!").AsColor("AAFF00"));
				}
				else if (hero.photonView.owner == PlayerTwo)
				{
					GameHelper.Broadcast((GExtensions.AsString(PlayerOne.customProperties[PhotonPlayerProperty.Name]).NGUIToUnity().AsColor("FFFFFF") + " wins!").AsColor("AAFF00"));
				}
				FengGameManagerMKII.Instance.FinishGame();
			}
		}

		public override void OnTitanKilled(TITAN titan, PhotonPlayer killer, int damage)
		{
			if (killer == PlayerOne)
			{
				SpawnTitan(1, titan);
			}
			else if (killer == PlayerTwo)
			{
				SpawnTitan(0, titan);
			}
			else
			{
				SpawnTitan(titan.transform.position, titan);
			}
		}

		private TITAN SpawnTitan(byte side, TITAN originalTitan = null)
		{
			Vector3 position = default(Vector3);
			switch (side)
			{
			case 0:
				position = new Vector3(MathHelper.RandomInt(LeftMinX.Value, LeftMaxX.Value), GroundLevel.Value, MathHelper.RandomInt(LeftMinZ.Value, LeftMaxZ.Value));
				break;
			case 1:
				position = new Vector3(MathHelper.RandomInt(RightMinX.Value, RightMaxX.Value), GroundLevel.Value, MathHelper.RandomInt(RightMinZ.Value, RightMaxZ.Value));
				break;
			}
			return SpawnTitan(position, originalTitan);
		}

		private TITAN SpawnTitan(Vector3 position, TITAN originalTitan = null)
		{
			GameObject gameObject = FengGameManagerMKII.Instance.SpawnTitanRaw(position, Quaternion.identity);
			if (originalTitan != null)
			{
				TitanClass abnormalType = originalTitan.abnormalType;
				gameObject.GetComponent<TITAN>().setAbnormalType2(abnormalType, abnormalType.Equals(TitanClass.Crawler));
			}
			else
			{
				gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Normal, forceCrawler: false);
			}
			return gameObject.GetComponent<TITAN>();
		}
	}
}

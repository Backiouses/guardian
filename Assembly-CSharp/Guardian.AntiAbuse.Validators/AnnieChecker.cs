namespace Guardian.AntiAbuse.Validators
{
	internal class AnnieChecker
	{
		public static bool IsAnimationPlayValid(FEMALE_TITAN annie, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || (info != null && annie.photonView.ownerId == info.sender.Id))
			{
				return true;
			}
			GuardianClient.Logger.Error("'FEMALE_TITAN.netPlayAnimation' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsAnimationSeekedPlayValid(FEMALE_TITAN annie, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || (info != null && annie.photonView.ownerId == info.sender.Id))
			{
				return true;
			}
			GuardianClient.Logger.Error("'FEMALE_TITAN.netPlayAnimationAt' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsCrossFadeValid(FEMALE_TITAN annie, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || (info != null && annie.photonView.ownerId == info.sender.Id))
			{
				return true;
			}
			GuardianClient.Logger.Error("'FEMALE_TITAN.netCrossFade' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}
	}
}

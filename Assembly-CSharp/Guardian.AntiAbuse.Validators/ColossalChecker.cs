namespace Guardian.AntiAbuse.Validators
{
	internal class ColossalChecker
	{
		public static bool IsRemovalValid(PhotonMessageInfo info)
		{
			if (info == null)
			{
				return true;
			}
			GuardianClient.Logger.Error($"'COLOSSAL_TITAN.removeMe' from #{info.sender.Id}.");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsAnimationPlayValid(COLOSSAL_TITAN ct, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || (info != null && ct.photonView.ownerId == info.sender.Id))
			{
				return true;
			}
			GuardianClient.Logger.Error("'COLOSSAL_TITAN.netPlayAnimation' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsAnimationSeekedPlayValid(COLOSSAL_TITAN ct, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || (info != null && ct.photonView.ownerId == info.sender.Id))
			{
				return true;
			}
			GuardianClient.Logger.Error("'COLOSSAL_TITAN.netPlayAnimationAt' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsCrossFadeValid(COLOSSAL_TITAN ct, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || (info != null && ct.photonView.ownerId == info.sender.Id))
			{
				return true;
			}
			GuardianClient.Logger.Error("'COLOSSAL_TITAN.netCrossFade' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsDoorChangeValid(COLOSSAL_TITAN ct, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || (info != null && ct.photonView.ownerId == info.sender.Id))
			{
				return true;
			}
			GuardianClient.Logger.Error("'COLOSSAL_TITAN.changeDoor' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}
	}
}

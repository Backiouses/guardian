namespace Guardian.AntiAbuse.Validators
{
	internal class TitanChecker
	{
		public static bool IsTitanTypeSetValid(TITAN titan, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || info == null || titan.photonView.ownerId == info.sender.Id || info.sender.isMasterClient)
			{
				return true;
			}
			GuardianClient.Logger.Error("'TITAN.netSetAbnormalType' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsCrossFadeValid(TITAN titan, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || (info != null && titan.photonView.ownerId == info.sender.Id))
			{
				return true;
			}
			GuardianClient.Logger.Error("'TITAN.netCrossFade' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsAnimationPlayValid(TITAN titan, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || (info != null && titan.photonView.ownerId == info.sender.Id))
			{
				return true;
			}
			GuardianClient.Logger.Error("'TITAN.netPlayAnimation' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsAnimationSeekedPlayValid(TITAN titan, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || (info != null && titan.photonView.ownerId == info.sender.Id))
			{
				return true;
			}
			GuardianClient.Logger.Error("'TITAN.netPlayAnimationAt' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsTargetSetValid(TITAN titan, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || (info != null && titan.photonView.ownerId == info.sender.Id))
			{
				return true;
			}
			GuardianClient.Logger.Error("'TITAN.setMyTarget' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsSoundPlayValid(TITAN titan, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || info == null || titan.photonView.ownerId == info.sender.Id)
			{
				return true;
			}
			GuardianClient.Logger.Error("'TITAN.playsoundRPC' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsRightGrabValid(TITAN titan, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || info == null || titan.photonView.ownerId == info.sender.Id)
			{
				return true;
			}
			GuardianClient.Logger.Error("'TITAN.grabToRight' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsLeftGrabValid(TITAN titan, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || info == null || titan.photonView.ownerId == info.sender.Id)
			{
				return true;
			}
			GuardianClient.Logger.Error("'TITAN.grabToLeft' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsLevelSetValid(TITAN titan, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || (info != null && (titan.photonView.ownerId == info.sender.Id || info.sender.isMasterClient)))
			{
				return true;
			}
			GuardianClient.Logger.Error("'TITAN.netSetLevel' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}
	}
}
